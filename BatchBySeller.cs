
public class BatchBySellerReportMeta
{   
    public string ItemNumber{get;set;}
    
    public string Description{get;set;}
    
    public string TotalQty{get;set;}
    
    public string Zone{get;set;}
    
    public string SONumber{get;set;}
    
    public string SOQty{get;set;}
}

public class SOInfo
{
    public SOInfo()
    {
        this.ItemTransactions = new List<ItemTransaction>();
    }
    
    public SONumber{get;set;}
    
    public List<ItemTransaction> ItemTransactions{get;set;}
    
    public void AddItemTransaction(ItemTransaction item)
    {
        this.ItemTransactions.Add(item);    
    }
    
    public int GetItemQty(string itemNumber)
    {
        var itemTrans = this.ItemTransactions.FirtOrDefault(item => item.ItemInfo.ItemNumber == itemNumber);
        if(itemTrans == null)
        {
            return 0;
        }
        return itemTrans.Quantity;
    }
}

public class ItemInfo()
{
    public ItemInfo(string itemNumber, string description)
    {
        this.ItemNumber = itemNumber;
        this.Description = description;
    }
    
    public string ItemNumber{get;set;}
    
    public string Description{get;set;}
    
    public override bool Equals (object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        ItemInfo item = obj as ItemInfo;
    
        return this.ItemNumber == item.ItemNumber && this.Description == item.Description;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        return this.ItemNumber.GeHashCode() ^ this.Description.GetHashCode();
    }
}

public class LPNInfo
{
    public string Location{get;set;}
    
    public string LPN{get;set;}
    
    public int Quantity{get;set;}
}

public class ItemTransaction
{
    public ItemTransaction(string itemNumber string description)
    {
        this.ItemInfo = new ItemInfo(itemNumber, description);
        this.LpnList = new List<LPNInfo>();
    }
    
    public SOInfo{get;set;}
    
    public ItemInfo{get;set;}
    
    public int Quantity{get;set;}
    
    public List<LPNInfo> LpnList{get;set;}
    
    public void AddLpn(LPNInfo lpnInfo)
    {
        this.LpnList.Add(lpnInfo);
    }
    
    public void SetSOInfo(SOInfo soInfo)
    {
        this.SOInfo = soInfo;
        this.SOInfo.AddItemTransaction()
    }
}


List<SOInfo> sos = new List<SOInfo>();
/*
*1. 按照Item分组
*2. 将Item需要的LPN按照Location分组
*3. 构造报表需要的数据格式：每个Meta代表为报表上的一行数据，报表上某些行不需要显示的数据，我们就不构造
*   例如：在显示Lpn信息的行上，我们就不构造ItemNumber、Description等信息
*4. 将SO信息补齐在Meta对象上
*/
List<BatchBySellerReportMeta> metaDatas = new List<BatchBySellerReportMeta>();
sos.SelectMany(so => so.ItemTransactions)
    .GroupBy(item => item.ItemInfo)
    .ToList()
    .ForEach(g => {
        var itemInfo = g.Key;
        var totalQty = g.ToList().Sum(itemTrans => itemTrans.Quantity); 
        
        var lpns = g.ToList().SelectMany(itemTrans => itemTrans.LpnList);
        var sos = g.ToList().Select(itemTrans => itemTrans.SOInfo);
        
        
        var lpnGroup = lpns.GroupBy(lpn => lpn.Location).ToList();
        List<BatchBySellerReportMeta> subMetaDatas = new List<BatchBySellerReportMeta>();
        for(int i=0;i<lpnGroup.Count;i++)
        {
            BatchBySellerReportMeta meta = new BatchBySellerReportMeta();
            if(i==0)
            {
                meta.ItemNumber = itemInfo.ItemNumber;
                meta.Description = itemInfo.Description;
                meta.TotalQty = totalQty;        
            }
            
            meta.Zone = "Loc:" + lpnGroup[i].Key;
            subMetaDatas.Add(meta);
            
            foreach(var lpn in lpnGroup[i].ToList())
            {
                subMetaDatas.Add(new BatchBySellerReportMeta
                {
                   Zone = string.Format("LPN:{0} X {1}" lpn.LPN, lpn.Quantity)
                });
            }
           
        }         
        for(int i=0; i<sos.count; i++)
        {
            subMetaDatas[i].SONumber = sos[i].SONumber;
            subMetaDatas[i].SOQty = sos[i].GetItemQty(itemInfo.ItemNumber);
        }
         
        metaDatas.AddRange(subMetaDatas);
    });





