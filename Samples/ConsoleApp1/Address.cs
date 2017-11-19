using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Nightingale;
using Nightingale.Entities;
using Nightingale.Extensions;
using Nightingale.Queries;

// generated Address.cs
		
namespace ConsoleApp1
{
	/// <summary>
	/// 
	/// </summary>
	[Table("")]
	[Description("")]
	public partial class Address : Entity
	{
	
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[Mandatory]
		
        [FieldType("Person")]
        [Cascade(Cascade.None)]
        
        [ForeignKey("Person")]
        public int FK_Person_ID { get; protected set; }

        public Person Person
        {
            get
            {
                if(_Person == null)
                {
                    _Person = Session?.Get(FK_Person_ID, typeof(Person)) as Person;
                }

                return _Person;
            }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Person), _Person, value);
                PropertyChangeTracker.AddPropertyChangedItem(nameof(FK_Person_ID), FK_Person_ID, value.Id);
                
                _Person = value;
                FK_Person_ID = value.Id;
                
                OnPropertyChanged();
            }
        }

        private Person _Person;          
        
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[Mandatory]
		
        [FieldType("bool")]
        [Cascade(Cascade.None)]
        
		public bool IsValid
        {
            get { return _IsValid; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(IsValid), _IsValid, value);
                _IsValid = value; 
                
                OnPropertyChanged();
            }
        }
        
        private bool _IsValid;
    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[Mandatory]
		
		[DateOnly]
		
        [FieldType("DateTime")]
        [Cascade(Cascade.None)]
        		
		public DateTime ValidFrom
        {
            get { return _ValidFrom; }
            set
            {
                var newValue = value.Date;
                PropertyChangeTracker.AddPropertyChangedItem(nameof(ValidFrom), _ValidFrom, newValue);
                _ValidFrom = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private DateTime _ValidFrom;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
        [FieldType("DateTime")]
        [Cascade(Cascade.None)]
        		
		public DateTime? ValidTo
        {
            get { return _ValidTo; }
            set
            {
                var newValue = value;
                PropertyChangeTracker.AddPropertyChangedItem(nameof(ValidTo), _ValidTo, newValue);
                _ValidTo = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private DateTime? _ValidTo;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[MaxLength(12)]
		
		[Mandatory]
		
        [FieldType("string")]
        [Cascade(Cascade.None)]
        
		public string Zip
        {
            get { return _Zip; }
            set
            {
                var newValue = value;
                if(newValue != null)
                    newValue = value.Substring(0, value.Length > 12 ? 12 : value.Length);
                
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Zip), _Zip, newValue);
                _Zip = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private string _Zip;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[MaxLength(50)]
		
		[Mandatory]
		
        [FieldType("string")]
        [Cascade(Cascade.None)]
        
		public string Town
        {
            get { return _Town; }
            set
            {
                var newValue = value;
                if(newValue != null)
                    newValue = value.Substring(0, value.Length > 50 ? 50 : value.Length);
                
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Town), _Town, newValue);
                _Town = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private string _Town;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[MaxLength(100)]
		
		[Mandatory]
		
        [FieldType("string")]
        [Cascade(Cascade.None)]
        
		public string Street
        {
            get { return _Street; }
            set
            {
                var newValue = value;
                if(newValue != null)
                    newValue = value.Substring(0, value.Length > 100 ? 100 : value.Length);
                
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Street), _Street, newValue);
                _Street = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private string _Street;
	    
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		[Mapped]
	  	
		[Mandatory]
		
        [FieldType("AddressType")]
        [Cascade(Cascade.None)]
        
        [Enum]            
        public AddressType Type
        {
            get { return _Type; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Type), _Type, value);
                _Type = value;
                
                OnPropertyChanged();
            }
        }
        
        private AddressType _Type;
        

		/// <summary>
		/// Initializes a new Address class.
		/// </summary>
		public Address()
        {
        
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        public override bool Validate()
        {
        
			if(Person == null)
				return false;    
            
			if(Zip == null)
				return false;    
            
			if(Town == null)
				return false;    
            
			if(Street == null)
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
	