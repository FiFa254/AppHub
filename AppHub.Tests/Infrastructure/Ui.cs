using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests.Infrastructure
{
    /// <summary>
    /// เข้าถึง control / event handler ที่เป็น private ของ form ผ่าน reflection
    /// </summary>
    internal static class Ui
    {
        private const BindingFlags Flags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>ดึง field (control) ตามชื่อ — ไล่ขึ้นไปถึง base class</summary>
        public static T Get<T>(object target, string fieldName)
        {
            for (var type = target.GetType(); type != null; type = type.BaseType)
            {
                var field = type.GetField(fieldName, Flags | BindingFlags.DeclaredOnly);
                if (field != null) return (T)field.GetValue(target);
            }
            throw new MissingFieldException(target.GetType().Name, fieldName);
        }

        /// <summary>เรียก method ตามชื่อ (ใช้กับ event handler / OnLoad)</summary>
        public static object Call(object target, string methodName, params object[] args)
        {
            MethodInfo method = null;
            for (var type = target.GetType(); type != null && method == null; type = type.BaseType)
                method = type.GetMethod(methodName, Flags | BindingFlags.DeclaredOnly);
            if (method == null)
                throw new MissingMethodException(target.GetType().Name, methodName);

            try
            {
                return method.Invoke(target, args);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        /// <summary>จำลองการกดปุ่ม — เรียก handler (sender, EventArgs.Empty)</summary>
        public static void Click(object form, string handlerName)
            => Call(form, handlerName, form, EventArgs.Empty);

        /// <summary>เลือกแถวใน grid ที่ column มีค่าตรงกับ value</summary>
        public static void SelectRow(DataGridView grid, string column, string value)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (Convert.ToString(row.Cells[column].Value) == value)
                {
                    grid.CurrentCell = row.Cells[column];
                    return;
                }
            }
            Assert.Fail($"ไม่พบแถว {column} = {value} ใน grid");
        }
    }
}
