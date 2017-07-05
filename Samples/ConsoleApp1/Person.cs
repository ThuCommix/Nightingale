using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Concordia.Framework;
using Concordia.Framework.Entities;
using Concordia.Framework.Extensions;
using Concordia.Framework.Queries;

// generated Person.cs
		
namespace ConsoleApp1
{
	/// <summary>
	/// 
	/// </summary>
	[Table("")]
	[Description("")]
	public class Person : Entity
	{
	
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[MaxLength(50)]
		
		[Mandatory]
		
        [FieldType("string")]
        [Cascade(Cascade.None)]
        
		public string FirstName
        {
            get { return _FirstName; }
            set
            {
                var newValue = value;
                if(newValue != null)
                    newValue = value.Substring(0, value.Length > 50 ? 50 : value.Length);
                
                PropertyChangeTracker.AddPropertyChangedItem(nameof(FirstName), _FirstName, newValue);
                _FirstName = newValue;
            }
        }
        
        private string _FirstName;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[MaxLength(50)]
		
		[Mandatory]
		
        [FieldType("string")]
        [Cascade(Cascade.None)]
        
		public string Name
        {
            get { return _Name; }
            set
            {
                var newValue = value;
                if(newValue != null)
                    newValue = value.Substring(0, value.Length > 50 ? 50 : value.Length);
                
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Name), _Name, newValue);
                _Name = newValue;
            }
        }
        
        private string _Name;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
        [FieldType("int")]
        [Cascade(Cascade.None)]
        
		public int? Age
        {
            get { return _Age; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Age), _Age, value);
                _Age = value;
            }
        }
        
        private int? _Age;
    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
        [FieldType("bool")]
        [Cascade(Cascade.None)]
        
		public bool? IsLegalAge
        {
            get { return _IsLegalAge; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(IsLegalAge), _IsLegalAge, value);
                _IsLegalAge = value; 
            }
        }
        
        private bool? _IsLegalAge;
    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
		[Cascade(Cascade.SaveDelete)]
        [FieldType("Address")]
        
        [ReferenceField("FK_Person_ID")]
		public EntityCollection<Address> Addresses
        {
            get
            {
                if(!_AddressesQueried)
                {
                    var query = Query.CreateQuery<Address>();
                    var group = query.CreateQueryConditionGroup();
                    
                    group.CreateQueryCondition<Address>(x => x.FK_Person_ID == Id);
                    group.CreateQueryCondition<Address>(x => x.Deleted == false);
                    
                    var items = Session?.ExecuteQuery(query)?.OfType<Address>();
                    if(items != null)
                    {
                        items.ForEach(x => x.PropertyChangeTracker.DisableChangeTracking = true);
                        _Addresses.AddRange(items);
                        items.ForEach(x => x.PropertyChangeTracker.DisableChangeTracking = false);
                        _AddressesQueried = true;   
                    }
                }
                
                return _Addresses;
            }
        }
        
        private EntityCollection<Address> _Addresses;
        private bool _AddressesQueried;
	
		/// <summary>
		/// $ {Name}, {FirstName} 
		/// </summary>
		[Description("")]
		[Expression("$ {Name}, {FirstName}")]
        [VirtualProperty]
        [FieldType("string")]
        public string FullName => $" {Name}, {FirstName}";
	
		/// <summary>
		/// Addresses.Where(x => x.IsValid).ToList() 
		/// </summary>
		[Description("")]
        [Expression("Addresses.Where(x => x.IsValid).ToList()")]
        [VirtualProperty]
        [FieldType("Address")]
        public List<Address> ValidAddresses => Addresses.Where(x => x.IsValid).ToList();
	

		/// <summary>
		/// Initializes a new Person class.
		/// </summary>
		public Person()
        {
            _Addresses = new EntityCollection<Address>(this, "Person");
		
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        public override bool Validate()
        {
        
			if(FirstName == null)
				return false;    
            
			if(Name == null)
				return false;    
            	

			return true;
		}
        
        protected override void EagerLoadProperties()
        {
            if(Session == null)
                return;
                
        
        }
	}
}	
	