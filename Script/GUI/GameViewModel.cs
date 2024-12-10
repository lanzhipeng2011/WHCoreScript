using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Item;
using GCGame.Table;
public class ViewModel
{
    public class PropertyList<T> 
    {
        List<T> PropertyValue;

        public List<T> Value 
        {
            get { return this.PropertyValue; }
            set 
            {
                this.PropertyValue = value;
                if (SetEvents != null) 
                {
                    for (int i = 0; i < SetEvents.Count;i++)
                    {
                        this.SetEvents[i](value);
                    }
                } 
            }
        }
        public void Add(T item) 
        {
            this.PropertyValue.Add(item);
            if(AddEvents!=null)
            {
                for (int i = 0; i < AddEvents.Count;i++)
                {
                    AddEvents[i](item);
                }
            }
        }

        public T this[int index] 
        {
            get { return this.PropertyValue[index]; }
            set
            {
                this.PropertyValue[index] = value; 
                if(this.SetItemValueEvents!=null)
                {
                    for (int i = 0; i < this.SetItemValueEvents.Count;i++)
                    {
                        this.SetItemValueEvents[i](value);
                    }
                }
            }
        }
        public void Remove(T item) 
        {
            this.PropertyValue.Remove(item);
            if (RemoveEvents != null)
            {
                for (int i = 0; i < RemoveEvents.Count; i++)
                {
                    RemoveEvents[i](item);
                }
            }
        }

        List<System.Action<T>> AddEvents;
        List<System.Action<T>> RemoveEvents;
        List<System.Action<T>> SetItemValueEvents;
        List<SetPropertyListValue> SetEvents;
        public delegate void SetPropertyListValue(List<T> v);
        public void AddItemEvent(System.Action<T> fn) 
        {
            if (this.AddEvents == null) 
            {
                this.AddEvents = new List<System.Action<T>>();
            }
            this.AddEvents.Add(fn);
        }
        public void RemoveEvent(System.Action<T> fn) 
        {
            if (this.RemoveEvents == null) 
            {
                this.RemoveEvents = new List<System.Action<T>>();
            }
            this.RemoveEvents.Add(fn);
        }

        public void SetItemValueEvent(System.Action<T> fn)
        {
            if (this.SetItemValueEvents == null)
            {
                this.SetItemValueEvents = new List<System.Action<T>>();
            }
            this.SetItemValueEvents.Add(fn);
        }
    }

    public class Property<T> 
    {
        T PropertyValue;
        public T Value
        {
            set
            {
                PropertyValue = value;
                this.Changed();
            }
            get { return PropertyValue; }
        }
        List<SetEvent> SetEvents;
        public delegate void SetEvent(T Value); 
        public void Changed() 
        {
            if (SetEvents != null)
            {
                foreach (SetEvent ev in SetEvents)
                {
                    ev(Value);
                }
            }
        }
        public void Binding(SetEvent fn)
        {
            if (SetEvents == null) 
            {
                SetEvents = new List<SetEvent>();
            }
            SetEvents.Add(fn); 
        }
        public void UnBinding(SetEvent fn) 
        {
            SetEvents.Remove(fn);
        }
    }
}
public class GameViewModel 
{
    static Dictionary<System.Type, ViewModel> viewModels;
    public static T Get<T>() where T:ViewModel,new()
    { 
       System.Type typ = typeof(T);
       if (viewModels == null) 
       {
           viewModels = new Dictionary<System.Type, ViewModel>();
       }
       if (!viewModels.ContainsKey(typ)) 
       {
           T newT = new T();
           viewModels[typ] = newT; 
       }
       return (T)viewModels[typ];
    }
}


public class PlayerGemDataViewModel : ViewModel 
{
    public PropertyList<int> GemsId;

    public int GetGemId(int EquipSlot, int Index) 
    {
      int totalIndex = EquipSlot * (int)GemSlot.OPEN_NUM + Index;
      return GemsId.Value[totalIndex];
    }
    public  PlayerGemDataViewModel() 
    {
        this.GemsId = new PropertyList<int>();
        this.GemsId.Value = new List<int>();
        for (int i = 0; i < (int)GemData.CONSTVALUE.SIZE; i++)
        {
            this.GemsId.Add(-1);
        }
    }
} 

public class MainPlayerViewModel :ViewModel
{
   public ViewModel.Property<int> AutoHpID = new Property<int>();
   public ViewModel.Property<int> AutoMpID = new Property<int>();

   public ViewModel.Property<bool> CanAutoHp = new Property<bool>();
   public ViewModel.Property<bool> CanAutoMp = new Property<bool>(); 
   
   public ViewModel.Property<float> HpItemCDTime = new Property<float>();
   public ViewModel.Property<float> MpItemCDTime = new Property<float>();

   int totalHPCDTime;
   public ViewModel.Property<float> HPCD = new Property<float>();
   int totalMPCDTime; 
   public ViewModel.Property<float> MPCD = new Property<float>();
   
   GameItemContainer BackPack;
   public MainPlayerViewModel() 
   {
       BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
       this.AutoHpID.Binding((n) => 
       { 
           CheckAutoHP(n);
           totalHPCDTime = this.GetMedicineItemTotalTime(AutoHpID.Value);
       });
       this.AutoMpID.Binding((n) => 
       {
           CheckAutoMP(n);
           totalMPCDTime = this.GetMedicineItemTotalTime(AutoMpID.Value);
       });
       this.HpItemCDTime.Binding((newValue) =>
       {
           this.HPCD.Value = newValue / totalHPCDTime;
       });
       this.MpItemCDTime.Binding((newValue) =>
       {
           this.MPCD.Value = newValue / totalHPCDTime;
       }); 

   }

   int GetMedicineItemTotalTime(int itemID) 
   {
       Tab_UsableItem _UsableInfo = TableManager.GetUsableItemByID(itemID, 0);
       if (_UsableInfo == null) return 0; 
       int nCoolId = _UsableInfo.CoolDownId;
       Tab_CoolDownTime _CDTimeInfo = TableManager.GetCoolDownTimeByID(nCoolId, 0);
       if (nCoolId == null) return 0;
       return  _CDTimeInfo.CDTime;
   }
   void CheckAutoHP(int itemID) 
   {
       BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
       if (itemID == -1 || BackPack.GetItemCountByDataId(itemID) <= 0)
       {
           this.CanAutoHp.Value = false;
       }
       else
       {
           this.CanAutoHp.Value = true;
       }
   }
   void CheckAutoMP(int itemID) 
   {
       BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
       if (itemID == -1 || BackPack.GetItemCountByDataId(itemID) <= 0)
       {
           this.CanAutoMp.Value = false;
       }
       else
       {
           this.CanAutoMp.Value = true;
       }
   }
  
   public void BackPackChange() 
   {
       CheckAutoHP(this.AutoHpID.Value);
       CheckAutoMP(this.AutoMpID.Value);
   }
}