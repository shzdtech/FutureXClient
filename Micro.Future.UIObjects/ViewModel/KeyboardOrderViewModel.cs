using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Reflection;

namespace Micro.Future.ViewModel
{
    public enum CloseChoiceType
    {
        All = 0,
        IgnorePlus,
        PlusOpen
    }

    public enum ActionChoiceType
    {
        Buy = 0,
        Sell,
        AllCancel
    }

    public class KeyboardOrderViewModel : ViewModelBase, ICloneable
    {
        public bool OneKeyOrderPermitted {get;set;}
        public bool OpenOnlyOneOrder {get;set;}
        public bool OpenOnlyOneDirectionOrder {get;set;}
        public CloseChoiceType CloseChoice { get; set; }
        public List<ButtonSetting> Buttons {get;set;}

        //public override string ToString()
        //{
        //    Type t = typeof(KeyboardOrderViewModel);
        //    StringBuilder sb = new StringBuilder();
        //    PropertyInfo[] properties = t.GetProperties();
        //    foreach (PropertyInfo property in properties)
        //    {
        //        sb.Append("[" + property.Name + "]" + property.GetValue(this, null).ToString() + ", ");
        //    }

        //    return sb.ToString();
        //}

        public object Clone()
        {
            object newObject = Activator.CreateInstance(this.GetType());
            System.Reflection.PropertyInfo[] property = newObject.GetType().GetProperties();
            System.Reflection.PropertyInfo[] thisproperty = this.GetType().GetProperties();
            for (int i = 0; i < thisproperty.Length; i++)
            {
                System.Reflection.MethodInfo mi = property[i].GetSetMethod();
                if (mi == null)
                    break;
                object propertyvalue = thisproperty[i].GetValue(this, null);
                if (propertyvalue is ICloneable)
                {
                    property[i].SetValue(newObject, ((ICloneable)propertyvalue).Clone(), null);
                }
                else
                {
                    property[i].SetValue(newObject, propertyvalue, null);
                }
            }
            return newObject;
        }
    }

    public class ButtonSetting: ICloneable
    {
        public int Num {get;set;}
        public int ActionChoice { get; set; }
        public int BidOrAskePrice { get; set; }
        //public bool Confirmed { get; set; }

        public object Clone()
        {
            object newObject = Activator.CreateInstance(this.GetType());
            System.Reflection.PropertyInfo[] property = newObject.GetType().GetProperties();
            System.Reflection.PropertyInfo[] thisproperty = this.GetType().GetProperties();
            for (int i = 0; i < thisproperty.Length; i++)
            {
                System.Reflection.MethodInfo mi = property[i].GetSetMethod();
                if (mi == null)
                    break;
                object propertyvalue = thisproperty[i].GetValue(this, null);
                if (propertyvalue is ICloneable)
                {
                    property[i].SetValue(newObject, ((ICloneable)propertyvalue).Clone(), null);
                }
                else
                {
                    property[i].SetValue(newObject, propertyvalue, null);
                }
            }
            return newObject;
        }

        //public override string ToString()
        //{
        //    Type t = typeof(ButtonSetting);
        //    StringBuilder sb = new StringBuilder();
        //    PropertyInfo[] properties = t.GetProperties();
        //    foreach (PropertyInfo property in properties)
        //    {
        //        sb.Append("[" + property.Name + "]" + property.GetValue(this, null).ToString() + ", ");
        //    }

        //    return sb.ToString();
        //}
    }
}
