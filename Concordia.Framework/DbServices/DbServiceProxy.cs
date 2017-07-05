using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Concordia.Framework.Queries;
using Concordia.Framework.Sessions;

namespace Concordia.Framework.DbServices
{
    internal class DbServiceProxy<T> : RealProxy where T : IDbService
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        protected Session Session { get; }

        /// <summary>
        /// Initializes a new DbServiceProxy class.
        /// </summary>
        /// <param name="session"></param>
        internal DbServiceProxy(Session session) : base(typeof(T))
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            Session = session;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCallMsg = msg as IMethodCallMessage;
            if (methodCallMsg != null)
                return HandleMethodCall(methodCallMsg);

            return new ReturnMessage(new InvalidOperationException("The procedure was not registered."), methodCallMsg);
        }

        private IMessage HandleMethodCall(IMethodCallMessage msg)
        {
            var methodInfo = typeof(T).GetMethods().FirstOrDefault(x => x.GetCustomAttribute<ProcedureAttribute>() != null && x.Name == msg.MethodName);
            var procedure = methodInfo.GetCustomAttribute<ProcedureAttribute>();
            var parameters = methodInfo.GetCustomAttributes<ParameterAttribute>().ToList();

            if (parameters.Count != msg.ArgCount)
                throw new InvalidOperationException("The amount of arguments does not match.");

            var executeFunc = Session.GetType().GetMethod("ExecuteFunc").MakeGenericMethod(typeof(T).GetMethod(msg.MethodName).ReturnType);
            var queryParameters = new List<QueryParameter>();

            if(parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    var parameter = parameters[i];
                    queryParameters.Add(new QueryParameter(parameter.Name, msg.Args[i], parameter.DbType, parameter.IsNullable, parameter.Size));
                }
            }

            var result = executeFunc.Invoke(Session, new object[] { procedure.Name, queryParameters.ToArray() });

            return new ReturnMessage(result, null, 0, msg.LogicalCallContext, msg);
        }
    }
}
