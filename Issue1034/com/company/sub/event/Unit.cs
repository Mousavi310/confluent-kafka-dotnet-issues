// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.7.7.5
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace com.company.sub.@event
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	public partial class Unit : ISpecificRecord
	{
		public static Schema _SCHEMA = Schema.Parse(@"{""type"":""record"",""name"":""Unit"",""namespace"":""com.company.sub.event"",""fields"":[{""name"":""unitNumber"",""doc"":""@required: false, @description: a specific unit number for an individual unit within a multi-dwelling unit, @examples: 1|101"",""default"":null,""type"":[""null"",""string""]},{""name"":""type"",""doc"":""@required: false, @description: the type of the unit, @examples: Apartment|Building"",""default"":null,""type"":[""null"",""string""]},{""name"":""story"",""doc"":""@required: false, @description: the story or floor number for the unit, @examples: 1|2|3"",""default"":null,""type"":[""null"",""string""]},{""name"":""fiberCount"",""doc"":""@required: false, @description: the number of fibers available at the unit, @examples: 1|4"",""default"":null,""type"":[""null"",""string""]}]}");
		/// <summary>
		/// @required: false, @description: a specific unit number for an individual unit within a multi-dwelling unit, @examples: 1|101
		/// </summary>
		private string _unitNumber;
		/// <summary>
		/// @required: false, @description: the type of the unit, @examples: Apartment|Building
		/// </summary>
		private string _type;
		/// <summary>
		/// @required: false, @description: the story or floor number for the unit, @examples: 1|2|3
		/// </summary>
		private string _story;
		/// <summary>
		/// @required: false, @description: the number of fibers available at the unit, @examples: 1|4
		/// </summary>
		private string _fiberCount;
		public virtual Schema Schema
		{
			get
			{
				return Unit._SCHEMA;
			}
		}
		/// <summary>
		/// @required: false, @description: a specific unit number for an individual unit within a multi-dwelling unit, @examples: 1|101
		/// </summary>
		public string unitNumber
		{
			get
			{
				return this._unitNumber;
			}
			set
			{
				this._unitNumber = value;
			}
		}
		/// <summary>
		/// @required: false, @description: the type of the unit, @examples: Apartment|Building
		/// </summary>
		public string type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}
		/// <summary>
		/// @required: false, @description: the story or floor number for the unit, @examples: 1|2|3
		/// </summary>
		public string story
		{
			get
			{
				return this._story;
			}
			set
			{
				this._story = value;
			}
		}
		/// <summary>
		/// @required: false, @description: the number of fibers available at the unit, @examples: 1|4
		/// </summary>
		public string fiberCount
		{
			get
			{
				return this._fiberCount;
			}
			set
			{
				this._fiberCount = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.unitNumber;
			case 1: return this.type;
			case 2: return this.story;
			case 3: return this.fiberCount;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.unitNumber = (System.String)fieldValue; break;
			case 1: this.type = (System.String)fieldValue; break;
			case 2: this.story = (System.String)fieldValue; break;
			case 3: this.fiberCount = (System.String)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
