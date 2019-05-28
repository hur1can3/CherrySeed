using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CherrySeed.Utils;

namespace CherrySeed.EntitySettings
{
    public class PrimaryKeySetting
    {
        //public List<string> PrimaryKeyNames { get; set; }
        public string PrimaryKeyName { get; set; }

        public PrimaryKeySetting()
        {

        }

        //public PrimaryKeySetting(List<string> primaryKeyNames)
        //{
        //    PrimaryKeyNames = primaryKeyNames;
        //}

        public PrimaryKeySetting(string primaryKeyName)
        {
            //PrimaryKeyNames = new List<string> { primaryKeyName };
            PrimaryKeyName = primaryKeyName;
        }
    }

    public class PrimaryKeySetting<T> : PrimaryKeySetting
    {
        public PrimaryKeySetting(Expression<Func<T, object>> primaryKeyMember)
            : base(ReflectionUtil.GetMemberName(primaryKeyMember))
        { }
    }
}