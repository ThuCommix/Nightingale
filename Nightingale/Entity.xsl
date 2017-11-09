<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="text" indent="yes" />
    
	<xsl:template match="Entity">using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Nightingale;
using Nightingale.Entities;
using Nightingale.Extensions;
using Nightingale.Queries;

// generated <xsl:value-of select="@Name" />.cs
		
namespace <xsl:value-of select="@Namespace" />
{
	/// &lt;summary&gt;
	/// <xsl:value-of select="@Description" />
	/// &lt;/summary&gt;
	[Table("<xsl:value-of select="@Table" />")]
	[Description("<xsl:value-of select="@Description" />")]
	public partial class <xsl:value-of select="@Name" /> : Entity<xsl:for-each select="Interfaces/Interface">, <xsl:value-of select="@Name" /></xsl:for-each>
	{
	<xsl:for-each select="Fields/Field">
		/// &lt;summary&gt;
		/// <xsl:value-of select="@Description" />
		/// &lt;/summary&gt;
		[Description("<xsl:value-of select="@Description" />")]
		[Mapped]
	  	<xsl:if test="@FieldType = 'string' and @MaxLength > 0">
		[MaxLength(<xsl:value-of select="@MaxLength" />)]
		</xsl:if>
		<xsl:if test="@Mandatory = 'true'">
		[Mandatory]
		</xsl:if>
		<xsl:if test="@Unique = 'true'">
		[Unique]
		</xsl:if>
		<xsl:if test="@DateOnly = 'true'">
		[DateOnly]
		</xsl:if>
        <xsl:if test="@EagerLoad = 'true'">
        [EagerLoad]
        </xsl:if>
        [FieldType("<xsl:value-of select="@FieldType" />")]
        [Cascade(Cascade.<xsl:value-of select="@Cascade" />)]
        <xsl:choose>
	    <xsl:when test="@FieldType = 'string'">
		public <xsl:value-of select="@FieldType" /><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get { return _<xsl:value-of select="@Name" />; }
            set
            {
                var newValue = value;
                if(newValue != null)
                    newValue = value<xsl:if test="@FieldType = 'string' and @MaxLength > 0">.Substring(0, value.Length > <xsl:value-of select="@MaxLength" /> ? <xsl:value-of select="@MaxLength" /> : value.Length)</xsl:if>;
                
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, newValue);
                _<xsl:value-of select="@Name" /> = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private <xsl:value-of select="@FieldType" /><xsl:text xml:space="preserve"> </xsl:text>_<xsl:value-of select="@Name" />;
	    </xsl:when>
	  <xsl:when test="@FieldType = 'int'">
		public <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get { return _<xsl:value-of select="@Name" />; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, value);
                _<xsl:value-of select="@Name" /> = value;
                
                OnPropertyChanged();
            }
        }
        
        private <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text>_<xsl:value-of select="@Name" />;
    </xsl:when>
		<xsl:when test="@FieldType = 'decimal'">
		<xsl:if test="@DecimalPrecision > 0">[Decimal(<xsl:value-of select="@DecimalPrecision" />, <xsl:value-of select="@DecimalScale" />)]</xsl:if>
		public <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get { return _<xsl:value-of select="@Name" />; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, value);
                _<xsl:value-of select="@Name" /> = value; 
                
                OnPropertyChanged();
            }
        }
        
        private <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text>_<xsl:value-of select="@Name" />;
    </xsl:when>
		<xsl:when test="@FieldType = 'bool'">
		public <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get { return _<xsl:value-of select="@Name" />; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, value);
                _<xsl:value-of select="@Name" /> = value; 
                
                OnPropertyChanged();
            }
        }
        
        private <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text>_<xsl:value-of select="@Name" />;
    </xsl:when>
		<xsl:when test="@FieldType = 'DateTime'">		
		public <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get { return _<xsl:value-of select="@Name" />; }
            set
            {
                var newValue = value<xsl:if test="@DateOnly = 'true'"><xsl:if test="@Mandatory = 'false'">?</xsl:if>.Date</xsl:if>;
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, newValue);
                _<xsl:value-of select="@Name" /> = newValue;
                
                OnPropertyChanged();
            }
        }
        
        private <xsl:value-of select="@FieldType" /><xsl:if test="@Mandatory = 'false'">?</xsl:if> <xsl:text xml:space="preserve"> </xsl:text>_<xsl:value-of select="@Name" />;
	    </xsl:when>
	    <xsl:otherwise>
        <xsl:choose>
        <xsl:when test="@Enum = 'true'">
        [Enum]            
        public <xsl:value-of select="@FieldType" /><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get { return _<xsl:value-of select="@Name" />; }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, value);
                _<xsl:value-of select="@Name" /> = value;
                
                OnPropertyChanged();
            }
        }
        
        private <xsl:value-of select="@FieldType" /> _<xsl:value-of select="@Name" />;
        </xsl:when>
        <xsl:otherwise>
        [ForeignKey("<xsl:value-of select="@Name" />")]
        public int<xsl:if test="@Mandatory = 'false'">?</xsl:if> FK_<xsl:value-of select="@Name" />_ID { get; protected set; }

        public <xsl:value-of select="@FieldType" /><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" />
        {
            get
            {
                if(_<xsl:value-of select="@Name" /> == null<xsl:if test="@Mandatory = 'false'">&amp;&amp; FK_<xsl:value-of select="@Name" />_ID != null</xsl:if>)
                {
                    _<xsl:value-of select="@Name" /> = Session?.Load(FK_<xsl:value-of select="@Name" />_ID<xsl:if test="@Mandatory = 'false'">.Value</xsl:if>, typeof(<xsl:value-of select="@FieldType" />)) as <xsl:value-of select="@FieldType" />;
                }

                return _<xsl:value-of select="@Name" />;
            }
            set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(<xsl:value-of select="@Name" />), _<xsl:value-of select="@Name" />, value);
                PropertyChangeTracker.AddPropertyChangedItem(nameof(FK_<xsl:value-of select="@Name" />_ID), FK_<xsl:value-of select="@Name" />_ID, value<xsl:if test="@Mandatory = 'false'">?</xsl:if>.Id);
                
                _<xsl:value-of select="@Name" /> = value;
                FK_<xsl:value-of select="@Name" />_ID = value<xsl:if test="@Mandatory = 'false'">?</xsl:if>.Id;
                
                OnPropertyChanged();
            }
        }

        private <xsl:value-of select="@FieldType" /><xsl:text xml:space="preserve"> </xsl:text>_<xsl:value-of select="@Name" />;          
        </xsl:otherwise>
        </xsl:choose>
	    </xsl:otherwise>
		</xsl:choose>	
	</xsl:for-each>

	<xsl:for-each select="ListFields/ListField">
		/// &lt;summary&gt;
		/// <xsl:value-of select="@Description" />
		/// &lt;/summary&gt;
		[Description("<xsl:value-of select="@Description" />")]
		[Mapped]
		[Cascade(Cascade.<xsl:value-of select="@Cascade" />)]
        [FieldType("<xsl:value-of select="@FieldType" />")]
        <xsl:if test="@EagerLoad = 'true'">
        [EagerLoad]
        </xsl:if>
        [ReferenceField("FK_<xsl:value-of select="@ReferenceField" />_ID")]
		public IList&lt;<xsl:value-of select="@FieldType" />&gt; <xsl:value-of select="@Name" />
        {
            get
            {
                lock (_<xsl:value-of select="@Name" />)
                {
                    if(!_<xsl:value-of select="@Name" />Queried)
                    {
                        var query = Query.CreateQuery&lt;<xsl:value-of select="@FieldType" />&gt;();
                        var group = query.CreateQueryConditionGroup();
                    
                        group.CreateQueryCondition&lt;<xsl:value-of select="@FieldType" />&gt;(x =&gt; x.FK_<xsl:value-of select="@ReferenceField" />_ID == Id);
                        group.CreateQueryCondition&lt;<xsl:value-of select="@FieldType" />&gt;(x =&gt; x.Deleted == false);
                    
                        var items = Session?.ExecuteQuery(query)?.OfType&lt;<xsl:value-of select="@FieldType" />&gt;();
                        if(items != null)
                        {
                            items.ForEach(x => x.PropertyChangeTracker.DisableChangeTracking = true);
                            ((EntityCollection&lt;<xsl:value-of select="@FieldType" />&gt;)_<xsl:value-of select="@Name" />).AddRange(items);
                            items.ForEach(x => x.PropertyChangeTracker.DisableChangeTracking = false);
                            _<xsl:value-of select="@Name" />Queried = true;   
                        }
                    }
                }
                
                return _<xsl:value-of select="@Name" />;
            }
        }
        
        private IList&lt;<xsl:value-of select="@FieldType" />&gt; _<xsl:value-of select="@Name" />;
        private bool _<xsl:value-of select="@Name" />Queried;
	</xsl:for-each>

	<xsl:for-each select="VirtualFields/VirtualField">
		/// &lt;summary&gt;
		/// <xsl:choose><xsl:when test="@Description = '' or not(@Description)"><xsl:value-of select="translate(@Expression, '&quot;', '')" /></xsl:when><xsl:otherwise><xsl:value-of select="@Description" /></xsl:otherwise></xsl:choose> 
		/// &lt;/summary&gt;
		[Description("<xsl:value-of select="@Description" />")]
		[Expression("<xsl:value-of select="translate(@Expression, '&quot;', '')" />")]
        [VirtualProperty]
        [FieldType("<xsl:value-of select="@FieldType" />")]
        public <xsl:value-of select="@FieldType" /><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="@Name" /> =&gt; <xsl:value-of select="@Expression" />;
	</xsl:for-each>

	<xsl:for-each select="VirtualListFields/VirtualListField">
		/// &lt;summary&gt;
		/// <xsl:choose><xsl:when test="@Description = '' or not(@Description)"><xsl:value-of select="translate(@Expression, '&quot;', '')" /></xsl:when><xsl:otherwise><xsl:value-of select="@Description" /></xsl:otherwise></xsl:choose> 
		/// &lt;/summary&gt;
		[Description("<xsl:value-of select="@Description" />")]
        [Expression("<xsl:value-of select="translate(@Expression, '&quot;', '')" />")]
        [VirtualProperty]
        [FieldType("<xsl:value-of select="@FieldType" />")]
        public List&lt;<xsl:value-of select="@FieldType" />&gt; <xsl:value-of select="@Name" /> =&gt; <xsl:value-of select="@Expression" />;
	</xsl:for-each>

		/// &lt;summary&gt;
		/// Initializes a new <xsl:value-of select="@Name" /> class.
		/// &lt;/summary&gt;
		public <xsl:value-of select="@Name" />()
        {
        <xsl:for-each select="ListFields/ListField">
        <xsl:text xml:space="preserve">    </xsl:text>_<xsl:value-of select="@Name" /> = new EntityCollection&lt;<xsl:value-of select="@FieldType" />&gt;(this, "<xsl:value-of select="@ReferenceField" />", "<xsl:value-of select="@Name" />");
		</xsl:for-each>
        }

        /// &lt;summary&gt;
        /// Validates the entity.
        /// &lt;/summary&gt;
        public override bool Validate()
        {
        <xsl:for-each select="Fields/Field"><xsl:if test="@Mandatory = 'true' and (not(@Enum) or @Enum = 'false')">
            <xsl:if test="not(@FieldType = 'int' or @FieldType = 'decimal' or @FieldType = 'bool' or @FieldType = 'DateTime')">
			if(<xsl:value-of select="@Name" /> == null)
				return false;    
            </xsl:if>
		</xsl:if></xsl:for-each>	

			return true;
		}
        
        protected override void EagerLoadProperties()
        {
            if(Session == null)
                return;
                
        <xsl:for-each select="Fields/Field">
        <xsl:if test="@EagerLoad = 'true' and (@FieldType != 'string' and @FieldType != 'int' and @FieldType != 'decimal' and @FieldType != 'bool' and @FieldType != 'DateTime')">
            <xsl:if test="@Mandatory = 'false'">    
            if(FK_<xsl:value-of select="@Name" />_ID != null)
            {
                _<xsl:value-of select="@Name" /> = Session.Load(FK_<xsl:value-of select="@Name" />_ID<xsl:if test="@Mandatory = 'false'">.Value</xsl:if>, typeof(<xsl:value-of select="@FieldType"/>)) as <xsl:value-of select="@FieldType"/>;
            }
            </xsl:if>
            <xsl:if test="@Mandatory = 'true'">
            _<xsl:value-of select="@Name" /> = Session.Load(FK_<xsl:value-of select="@Name" />_ID, typeof(<xsl:value-of select="@FieldType"/>)) as <xsl:value-of select="@FieldType"/>;
            </xsl:if>          
        </xsl:if>
        </xsl:for-each>

        <xsl:for-each select="ListFields/ListField">
        <xsl:if test="@EagerLoad = 'true'">
            var items<xsl:value-of select="@Name" /> = Session.ExecuteQuery(GetQuery&lt;<xsl:value-of select="@FieldType" />&gt;(x =&gt; x.FK_<xsl:value-of select="@ReferenceField" />_ID == Id)).OfType&lt;<xsl:value-of select="@FieldType" />&gt;();
            ((EntityCollection&lt;<xsl:value-of select="@FieldType" />&gt;)_<xsl:value-of select="@Name" />).AddRange(items<xsl:value-of select="@Name" />);
            _<xsl:value-of select="@Name" />Queried = true;
        </xsl:if>
        </xsl:for-each>
        }
	}
}	
	</xsl:template>
</xsl:stylesheet>