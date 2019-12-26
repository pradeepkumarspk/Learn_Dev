using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace Selenium_Docker.Database
{
    public class dbUtils
    {
        private readonly Selenium_Docker.Database.DAL _dal;

        public dbUtils(string dsn)
        {
            _dal = new Selenium_Docker.Database.DAL(dsn);
            
        }


        public List<string> selectAllocationBatchRun()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT * FROM [AllocationBatchRuns] ");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["AllocationBatchRunCode"].ToString().Trim());
            }
            return results;
        }

        public List<string> selectAllocationCarriers()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT * FROM [AllocationCarriers] ");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["AllocationCarrierCode"].ToString().Trim());
            }
            return results;
        }


        public string selectCancelReasonDesc(string reasonCode)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Description FROM [Codes] where codeId='CANCEL' and Code like '" + reasonCode + "%'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Description"].ToString().Trim());
            }
            return results[0].ToString();
        }

        // To set the Inventory qty for specific store only
        public void SetInvQtyForStore(string quantity, string vendorNo, string SKU)
        {
            ResetInvQtyToZeroExceptVendor(SKU, vendorNo);
            string itemId = GetItemId(SKU);
            string inventoryTable = GetInventoryTable();
            string age = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
            var sb = new StringBuilder();
            sb.AppendLine($"insert into[" + inventoryTable + "] values('" + vendorNo + "', '" + itemId + "', '" + quantity + "', '" + age + "')");
            var sql = sb.ToString();
            _dal.ExecuteSQL(sql);
        }

        // To set the Pick Ticket Id for specific store only
        public void SetPickTicketIdForStore(string vendorNo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE VendorNumbers SET Id = 0000 where VendorNo = '" + vendorNo + "' and Usage = '10202'");
            var sql = sb.ToString();
            _dal.ExecuteSQL(sql);
        }
        // To get the Pick Ticket Id for specific store only
        public string GetPickTicketIdForStore(string vendorNo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Id FROM VendorNumbers WHERE VendorNo = '" + vendorNo + "' and Usage = '10202'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Id"].ToString().Trim());
            }
            return results[0].ToString();
        }
        // Reset Inventory qty to zero for all store
        public void ResetInvQtyToZeroExceptVendor(string SKU, string vendorNo)
        {
            string itemId = GetItemId(SKU);
            string inventoryTable = GetInventoryTable();
            var sb = new StringBuilder();
            sb.AppendLine($"Delete From [" + inventoryTable + "] where ItemId = '" + itemId + "'");
            var sql = sb.ToString();
            _dal.ExecuteSQL(sql);
        }


        // to get the item id of the sku
        public string GetItemId(string SKU)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Select ItemId FROM [Items] where SKUNo = '" + SKU + "'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["ItemId"].ToString().Trim());
            }
            return results[0].ToString();
        }

        // to get the Inventory Table name
        public string GetInventoryTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Value FROM [SystemFlags] where Flag = 'INVENTORYTABLE'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Value"].ToString().Trim());
            }
            return results[0].ToString();
        }

        // to get the Vendor Name
        public string GetVendorName(string vendorNo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Name FROM [Vendors] where VendorNo = '" + vendorNo + "'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Name"].ToString().Trim());
            }
            return results[0].ToString();
        }

        // to get the Box Size
        public List<string> GetBozSize()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT BoxId FROM [Boxes] ");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["BoxId"].ToString().Trim());
            }
            return results;
        }
        //to get the box size and weight
        public Dictionary<string, Double> getBoxSizeAndWeight()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT BoxId, DunnageWeight FROM [Boxes]");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            Dictionary<string, Double> results = new Dictionary<string, Double>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["BoxId"].ToString().Trim(), Convert.ToDouble(ds.Tables[0].Rows[i]["DunnageWeight"].ToString().Trim()));
            }
            return results;
        }
        //to get Order weight from pndshipments table
        public List<string> getOrderWeight(string orderNum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Weight FROM [PndShipments] where Orderno='" + orderNum + "'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Weight"].ToString().Trim());
            }
            return results;

        }
        /// <summary>
        /// Deletes the records from AllocationConfigurationSettings table
        /// </summary>
        /// <param name="allocationOptionName"></param>
        public void DeleteAllocationConfigurationSettings(string allocationOptionName)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"delete AllocationConfigurationSettings where AllocationSettingCode in(" + allocationOptionName + ")");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
        }
        // To get Country Values from Codes table
        public List<string> GetCountryValues()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Select Code from [Codes] where CodeId='Country'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Code"].ToString().Trim());
            }
            return results;
        }


        // to get the Vendor Groups
        public List<string> GetGroups()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Distinct VendorGroup FROM [VendorGroups] ");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["VendorGroup"].ToString().Trim());
            }
            return results;
        }

        // to get the no of rows returned for PndShipments
        public int GetPndShipmentsRowCount(string orderNum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Select orderNo FROM [PndShipments] where orderNo = '" + orderNum + "'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            return ds.Tables[0].Rows.Count;
        }
        // to get the Host Update Type
        public List<string> GetHostUpdateType(string orderNum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Type FROM [HostUpdates] where orderNo = '" + orderNum + "'");
            var sql = sb.ToString().Trim();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Type"].ToString().Trim());
            }
            return results;
        }
        // To get OrderNos from PickTicketsOrders table
        public List<string> GetPickTicketsOrders()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Select Distinct OrderNo from [PickTicketsOrders] where OrderNo like 'UI%'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["OrderNo"].ToString().Trim());
            }
            return results;
        }
        // to get the tracking no of the order
        public string GetTrackingNo(string orderNum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Select TrackingNo FROM [PndShipments] where orderNo = '" + orderNum + "'");
            var sql = sb.ToString().Trim();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["TrackingNo"].ToString().Trim());
            }
            return results[0].ToString();
        }

        // to get the print Doc Items to be validated for the pick ticket no
        public List<string> GetPrintDocItems(string ticketNo, string item = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Select Top 1 pt." + item + " FROM [PickTicketsHeader] pth Inner Join [PickTickets] pt ON pth.PrintBatch = pt.PrintBatch where AltListNo = '" + ticketNo + "' order by " + item + " desc");
            var sql = sb.ToString().Trim();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["" + item + ""].ToString().Trim());
            }
            return results;
        }

        // To set the AllocationEnabled field for specific store only
        public void SetAllocationEnabledForStore(string vendorNo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE AllocationLocations SET AllocationEnabled = 1 where VendorNo = '" + vendorNo + "'");
            var sql = sb.ToString();
            _dal.ExecuteSQL(sql);
            sb.AppendLine($"UPDATE AllocationLocationCarriers SET AllocationCarrierEnabled = 1 where AllocationLocationId in (select allocationLocationId from AllocationLocations where VendorNo = '" + vendorNo + "')");
            sql = sb.ToString();
            _dal.ExecuteSQL(sql);
        }
        // To get the matching row from PndShipments table
        public List<string> checkPndShipments(string orderNo, int groupNumber)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT * FROM [PndShipments] where Orderno ='" + orderNo + "' AND TrackingNo is not NULL AND GroupNum=" + groupNumber + "");
            var sql = sb.ToString();
            List<String> results = new List<String>();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["GroupNum"].ToString().Trim());
            }
            return results;

        }
        // To get the matching row from Order Items table
        public List<string> checkOrderItems(string orderNo, int lineNum, int groupNum, int quantity)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT * FROM [OrderItems] WHERE OrderNo= '" + orderNo + "' AND LineNum= " + lineNum + " AND GroupNum= " + groupNum + " AND Quantity= " + quantity + "");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i].ToString().Trim());
            }
            return results;
        }

        //to get SKU weight from VendorItems table
        public Double getSKUWeight(string SKU)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT Weight FROM [VendorItems] where SKUNo='" + SKU + "'");
            var sql = sb.ToString();
            System.Data.DataSet ds = _dal.ExecuteSQLSelect(sql);
            List<String> results = new List<String>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                results.Add(ds.Tables[0].Rows[i]["Weight"].ToString().Trim());
            }
            return Convert.ToDouble(results[0]);

        }

    }
}
