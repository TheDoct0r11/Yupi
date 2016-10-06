﻿// ---------------------------------------------------------------------------------
// <copyright file="MigrationConfiguration.cs" company="https://github.com/sant0ro/Yupi">
//   Copyright (c) 2016 Claudio Santoro, TheDoctor
// </copyright>
// <license>
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </license>
// ---------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.NHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace Yupi.Model
{
    public class MigrationConfiguration : MigrationConfigurationBase
    {
        public MigrationConfiguration ()
        {
            MigrationAssembly = typeof (ORMConfiguration).Assembly;
            MigrationNamespace = "Yupi.Model.Db.Migrations";
        }

        protected override Configuration GetConfiguration ()
        {
            return ModelHelper.GetConfig ();
        }

        protected override List<MigrationExpressionBase> GetFromExpressions ()
        {
            var lastMigration = MigrationAssembly.DefinedTypes.Where (t => t.BaseType == typeof (Migration))
                .Where (s => HasConfigurationData (s))
                .OrderBy (t => GetVersion (t))
                .LastOrDefault ();

            if (lastMigration == null) {
                return new List<MigrationExpressionBase> ();
            }
            var f = lastMigration.GetField ("ConfigurationData", BindingFlags.Public | BindingFlags.Static);
            var data = (string)f.GetValue (null);
            return DeserializeConfiguration (data);
        }

        private bool HasConfigurationData (Type type)
        {
            return type.GetField ("ConfigurationData", BindingFlags.Public | BindingFlags.Static) != null;
        }

        private long GetVersion (Type type)
        {
            return type.GetCustomAttributes (false)
                .OfType<MigrationAttribute> ()
                .Select (x => x.Version)
                .FirstOrDefault ();
        }
    }
}