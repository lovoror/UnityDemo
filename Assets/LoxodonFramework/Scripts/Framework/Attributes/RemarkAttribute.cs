﻿using System;

namespace Loxodon.Framework.Attributes
{
    public class RemarkAttribute:Attribute
    {
        private string remark;
        public RemarkAttribute(string remark)
        {
            this.remark = remark;
        }

        public string Remark { get { return this.remark; } }
    }
}
