﻿using System;

namespace Nightingale.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CascadeAttribute : Attribute
	{
		public Cascade Cascade { get; }

		public CascadeAttribute(Cascade cascade)
		{
			Cascade = cascade;
		}
	}
}
