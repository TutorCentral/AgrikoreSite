using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters; // also add Framework reference 'System.Runtime.Serialization'
using System.Web.Script.Serialization;
using System.Threading;
using BlockChainExplorer.Models;

namespace BlockChainExplorer.Controllers
{
    public class BitcoinBlockCountObject
    {
        public long blockCount { get; set; }
    }

    public class BitcoinBlockInfoObject
    {
        public Info info { get; set; }
    }
    public class Info
    {
        public int version { get; set; }
        public int protocolversion { get; set; }
        public long blocks { get; set; }
        public int timeoffset { get; set; }
        public int connections { get; set; }
        public string proxy { get; set; }
        public double difficulty { get; set; }
        public bool testnet { get; set; }
        public double relayfee { get; set; }
        public string errors { get; set; }
        public string network { get; set; }
    }


    public class BitcoinBlockObject
    {
        public string blockHash { get; set; }
    }
    public class BitcoinTransactionObject
    {
        public string tx { get; set; }
    }
    public class BitcoinBlockDataObject
    {
        public string hash { get; set; }
        public long size { get; set; }
        public long height { get; set; }
        public long version { get; set; }
        public string merkleroot { get; set; }
        public List<string> tx { get; set; }
        public string time { get; set; }
        public string bits { get; set; }
        public double difficulty { get; set; }
        public string chainwork { get; set; }
        public long confirmations { get; set; }
        public string previousblockhash { get; set; }
        public string nextblockhash { get; set; }
        public double reward { get; set; }
        public bool isMainChain { get; set; }
    }
    public class AddressAndAmount
    {
        public double myamount { get; set; }
        public string myaddress { get; set; }
        public string inputoutput { get; set; }
    }

    public class ScriptSig
    {
        public string asm { get; set; }
        public string hex { get; set; }
    }

    public class Vin
    {
        public string txid { get; set; }
        public int vout { get; set; }
        public ScriptSig scriptSig { get; set; }
        public object sequence { get; set; }
        public int n { get; set; }
        public string addr { get; set; }
        public long valueSat { get; set; }
        public double value { get; set; }
        public object doubleSpentTxID { get; set; }
    }

    public class ScriptPubKey
    {
        public string hex { get; set; }
        public string asm { get; set; }
        public List<string> addresses { get; set; }
        public string type { get; set; }
    }

    public class Vout
    {
        public string value { get; set; }
        public int n { get; set; }
        public ScriptPubKey scriptPubKey { get; set; }
        public string spentTxId { get; set; }
        public int? spentIndex { get; set; }
        public int? spentHeight { get; set; }
    }

    public class BitcoinTransactionDataObject
    {
        public string txid { get; set; }
        public int version { get; set; }
        public int locktime { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
        public string blockhash { get; set; }
        public int blockheight { get; set; }
        public int confirmations { get; set; }
        public int time { get; set; }
        public int blocktime { get; set; }
        public double valueOut { get; set; }
        public int size { get; set; }
        public double valueIn { get; set; }
        public double fees { get; set; }
    }

    public class BitcoinAddressTransactions
    {
        public string addrStr { get; set; }
        public double balance { get; set; }
        public long balanceSat { get; set; }
        public double totalReceived { get; set; }
        public long totalReceivedSat { get; set; }
        public double totalSent { get; set; }
        public long totalSentSat { get; set; }
        public double unconfirmedBalance { get; set; }
        public long unconfirmedBalanceSat { get; set; }
        public long unconfirmedTxApperances { get; set; }
        public long txApperances { get; set; }
        public List<string> transactions { get; set; }
    }

    public class BitcoinBlockchainController : Controller
    {
        private const string URL = "https://blockexplorer.com/api/block-index/";
        private const string blockURL = "https://blockexplorer.com/api/block/";
        private const string transactionURL = "https://blockexplorer.com/api/tx/";
        private const string addressURL = "https://blockexplorer.com/api/addr/";

        public static void CopyTodaysAddressesToYesterdays(Dictionary<string, long> myTodaysDictionary, Dictionary<string, long> myYesterdaysDictionary)
        {
            myYesterdaysDictionary.Clear();
            foreach (var myKey in myTodaysDictionary)
            {
                myYesterdaysDictionary.Add(myKey.Key, myKey.Value);
            }
            myTodaysDictionary.Clear();
        }
        public static void AddToDictionaryContainsKey(Dictionary<string, long> myDictionary, string myKey)
        {
            long myValue = 1;
            if (myDictionary.ContainsKey(myKey))
            {
                myDictionary[myKey] = myDictionary[myKey] + myValue;
            }
            else
            {
                myDictionary.Add(myKey, myValue);
            }
        }
        private static void GetBlockAddressesAndAmounts(ref List<AddressAndAmount> myaddresses, ref double valueIn, ref double valueOut, ref double fees, ref string sblocktime, string urlParameters)
        {
            string voutValue = "";
            double voutValueDouble = 0;
            //Console.WriteLine("URL to get block transactions from : {0}", urlParameters);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameters);
            request.Method = "GET";
            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            //Console.Out.WriteLine(response);

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            BitcoinTransactionDataObject d = js.Deserialize<BitcoinTransactionDataObject>(response);
                            //Console.WriteLine("blockHash: {0}", d.hash);
                            valueIn = d.valueIn;
                            valueOut = d.valueOut;
                            fees = d.fees;
                            sblocktime = d.blocktime.ToString();
                            if (d.vin != null)
                            {
                                foreach (var item in d.vin)
                                {
                                    if (item.addr != null)
                                    {
                                        AddressAndAmount anAddress = new AddressAndAmount();
                                        anAddress.myaddress = item.addr;
                                        anAddress.myamount = item.value;
                                        anAddress.inputoutput = "Input";
                                        myaddresses.Add(anAddress);
                                    }
                                }
                            }
                            if (d.vout != null)
                            {
                                foreach (var item in d.vout)
                                {
                                    voutValue = item.value;
                                    if (item.scriptPubKey != null)
                                    {
                                        if (item.scriptPubKey.addresses != null)
                                        {
                                            if (item.scriptPubKey.addresses.Count > 0)
                                            {
                                                foreach (var itemb in item.scriptPubKey.addresses)
                                                {
                                                    try
                                                    {
                                                        voutValueDouble = Convert.ToDouble(voutValue);
                                                    }
                                                    catch
                                                    {
                                                        voutValueDouble = 0.0;
                                                    }
                                                    AddressAndAmount anAddress = new AddressAndAmount();
                                                    if (itemb != null) anAddress.myaddress = itemb;
                                                    anAddress.myamount = voutValueDouble;
                                                    anAddress.inputoutput = "Output";
                                                    myaddresses.Add(anAddress);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
        }

        private static void GetBlockAddresses(ref List<string> myaddresses, string urlParameters)
        {
            //Console.WriteLine("URL to get block transactions from : {0}", urlParameters);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameters);
            request.Method = "GET";
            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            //Console.Out.WriteLine(response);

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            BitcoinTransactionDataObject d = js.Deserialize<BitcoinTransactionDataObject>(response);
                            //Console.WriteLine("blockHash: {0}", d.hash);
                            if (d.vin != null)
                            {
                                foreach (var item in d.vin)
                                {
                                    if (item.addr != null)
                                        myaddresses.Add(item.addr);
                                }
                            }
                            if (d.vout != null)
                            {
                                foreach (var item in d.vout)
                                {
                                    if (item.scriptPubKey != null)
                                    {
                                        if (item.scriptPubKey.addresses != null)
                                        {
                                            if (item.scriptPubKey.addresses.Count > 0)
                                            {
                                                foreach (var itemb in item.scriptPubKey.addresses)
                                                {
                                                    if (itemb != null)
                                                        myaddresses.Add(itemb);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
        }

        private static void GetBlockTransactions(ref List<string> mytransactions, ref string nextblockHash, ref string sblocktime, ref long blocksize, string urlParameters)
        {
            //Console.WriteLine("URL to get block transactions from : {0}", urlParameters);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameters);
            request.Method = "GET";
            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            //Console.Out.WriteLine(response);

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            BitcoinBlockDataObject d = js.Deserialize<BitcoinBlockDataObject>(response);
                            //Console.WriteLine("blockHash: {0}", d.hash);
                            if (d.tx != null)
                            {
                                mytransactions = d.tx;
                            }
                            nextblockHash = d.nextblockhash;
                            sblocktime = d.time;
                            blocksize = d.size;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
        }

        private static void GetBlockAddressInfo(ref List<string> mytransactions, ref double balance, ref double totalReceived, ref double totalSent, string urlParameters = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameters);
            request.Method = "GET";

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            BitcoinAddressTransactions d = js.Deserialize<BitcoinAddressTransactions>(response);
                            //Console.WriteLine("blockHash: {0}", d.blockHash);
                            balance = d.balance;
                            totalReceived = d.totalReceived;
                            totalSent = d.totalSent;
                            mytransactions = d.transactions;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }

        }

        private static void GetBlockCount(ref long iblockcount, string urlParameters = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameters);
            request.Method = "GET";

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            BitcoinBlockCountObject d = js.Deserialize<BitcoinBlockCountObject>(response);
                            //Console.WriteLine("blockHash: {0}", d.blockHash);
                            iblockcount = d.blockCount;

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }

        }


        private static void GetBlockHash(ref string sBlockHash, string urlParameters = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameters);
            request.Method = "GET";
            //request.ContentType = "application/json";
            //request.ContentLength = urlParameters.Length;
            //using (Stream webStream = request.GetRequestStream())
            //using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
            //{
            //    requestWriter.Write(urlParameters);
            //}

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            //Console.Out.WriteLine(response);
                            // Parse the response body. Blocking!

                            //for (int i = 0; i < webResponse.Headers.Count; ++i)
                            //    Console.WriteLine("\nHeader Name:{0}, Header value :{1}", webResponse.Headers.Keys[i], webResponse.Headers[i]);

                            //DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(InputGpioPort));
                            //DataObject objResponse = (DataObject)dataContractJsonSerializer.ReadObject(webStream);

                            //JavaScriptSerializer js = new JavaScriptSerializer();
                            //var objText = reader.ReadToEnd();
                            //MyObject myojb = (MyObject)js.Deserialize(objText, typeof(MyObject));


                            JavaScriptSerializer js = new JavaScriptSerializer();
                            BitcoinBlockObject d = js.Deserialize<BitcoinBlockObject>(response);
                            //Console.WriteLine("blockHash: {0}", d.blockHash);
                            sBlockHash = d.blockHash;


                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }

        }

        public static DateTime UnixTimeStampToDateTime(string sUnixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            long unixTimeStamp = Convert.ToInt64(sUnixTimeStamp);
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        // GET: BitcoinBlockchain
        public ActionResult Index(BitcoinBlockchain model = null, string submitButton = null, string blocknumber = null, string lastbitcoinblock = null, string sthetransaction = null, string stheaddress = null)
        {
            //(0) Get the blockHash
            string urlParameters = "https://blockexplorer.com/api/status?q=getBlockCount";
            long latestblockcount = 474010;
            BitcoinBlockchainController.GetBlockCount(ref latestblockcount, urlParameters);
            model.latestblock = latestblockcount;

            try
            {

                if (submitButton == "Submit Block Number")
                {

                    int inumtransactions = 0;
                    long istartblock = 0;
                    long ilastblock = 0;
                    double valueIn = 0;
                    double valueOut = 0;
                    double fees = 0;
                    blocknumber = Request.QueryString["blocknumber"];

                    lastbitcoinblock = Request.QueryString["lastbitcoinblock"];
                    bool result = false;
                    result = Int64.TryParse(blocknumber, out istartblock);
                    result = Int64.TryParse(lastbitcoinblock, out ilastblock);

                    model.blocknumber = istartblock;
                    model.latestblock = ilastblock;
                    model.blockchaininfo = "";
                    string soutput = "Enter a Bitcoin block number within the range 0 and " + ilastblock.ToString();

                    sthetransaction = Request.QueryString["transactionid"];

                    stheaddress = Request.QueryString["walletid"];
                    model.transactionid = sthetransaction;
                    model.walletid = stheaddress;


                    if ((istartblock >= 0) && (istartblock <= ilastblock))
                    {
                        //(1) Get the blockHash
                        urlParameters = URL + istartblock.ToString();
                        string sBlockHash = "";
                        BitcoinBlockchainController.GetBlockHash(ref sBlockHash, urlParameters);

                        //(2) Get the block transactions
                        //int iblockcount = istartblock;
                        string nextblockHash = "";
                        string sblocktime = "";
                        long blocksize = 0;
                        DateTime dblockdate = System.DateTime.Now;
                        urlParameters = blockURL + sBlockHash;
                        List<string> mytransactions;
                        mytransactions = new List<string>();
                        BitcoinBlockchainController.GetBlockTransactions(ref mytransactions, ref nextblockHash, ref sblocktime, ref blocksize, urlParameters);

                        dblockdate = UnixTimeStampToDateTime(sblocktime);
                        soutput = "<b>Block: " + istartblock.ToString() + " : " + sBlockHash.ToString() + "</br> Time: " + dblockdate.ToString() + "</b> Size:" + blocksize.ToString() + "</br> NumTransactions:" + mytransactions.Count().ToString() + "</br>";
                        //sw.WriteLine(iblockcount.ToString() + "," + sBlockHash + "," + mytransactions.Count().ToString() + "," + dblockdate.ToString());

                        //Get the tramsactions
                        if ((mytransactions != null))
                        {
                            foreach (var item in mytransactions)
                            {
                                inumtransactions++;
                                string sthelocaltransaction = item.ToString();
                                urlParameters = transactionURL + sthelocaltransaction;
                                List<AddressAndAmount> myaddresses;
                                myaddresses = new List<AddressAndAmount>();
                                BitcoinBlockchainController.GetBlockAddressesAndAmounts(ref myaddresses, ref valueIn, ref valueOut, ref fees, ref sblocktime, urlParameters);
                                soutput = soutput + "<font color=orange size=2>" + inumtransactions.ToString() + ". Transaction Amount Input: " + valueIn.ToString() + ", Amount Output: " + valueOut.ToString() + ", Fees " + fees.ToString() + "</font></br>";
                                soutput = soutput + "Transaction ID:";
                                soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Transaction ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthelocaltransaction + "&walletid=" + stheaddress + Convert.ToChar(34) + ">";
                                soutput = soutput + "<u>" + sthelocaltransaction + "</u></a></br>";
                                //sw.WriteLine("{0}. Transaction Amount Input: {1}, Amount Output: {2}, Fees {3}", inumtransactions.ToString(), valueIn.ToString(), valueOut.ToString(), fees.ToString());
                                foreach (var itemb in myaddresses)
                                {
                                    soutput = soutput + "<font color=blue size=1>Address: </font>";
                                    soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Wallet ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthetransaction + "&walletid=" + itemb.myaddress.ToString() + Convert.ToChar(34) + ">";
                                    soutput = soutput + "<font color=blue size=1><u>" + itemb.myaddress.ToString() + "</u></font></a>";
                                    soutput = soutput + "<font color=blue size=1>" + ", " + itemb.inputoutput + " Amount: " + itemb.myamount.ToString() + "</font></br>";
                                }
                                myaddresses.Clear();

                            }
                        }
                    }
                    model.blockchaininfo = "<html><head></head><body><p>" + soutput + "</p></body></html>";

                    return View(model);
                    //return RedirectToAction("Index");
                }
                else if (submitButton == "Submit Transaction ID")
                {
                    long ilastblock = 0;
                    long istartblock = 0;
                    blocknumber = Request.QueryString["blocknumber"];

                    lastbitcoinblock = Request.QueryString["lastbitcoinblock"];

                    bool result = false;
                    result = Int64.TryParse(blocknumber, out istartblock);
                    result = Int64.TryParse(lastbitcoinblock, out ilastblock);
                    model.blocknumber = istartblock;
                    model.latestblock = ilastblock;

                    model.blockchaininfo = "";
                    string soutput = "Enter a transaction ID";
                    double valueIn = 0;
                    double valueOut = 0;
                    double fees = 0;
                    AddressAndAmount anAddress = new AddressAndAmount();

                    sthetransaction = Request.QueryString["transactionid"];

                    stheaddress = Request.QueryString["walletid"];

                    model.transactionid = sthetransaction;
                    model.walletid = stheaddress;

                    if (sthetransaction != "")
                    {
                        soutput = "<b>Transaction: " + sthetransaction + "</b></br>";
                        urlParameters = transactionURL + sthetransaction;
                        string sblocktime = "";
                        List<AddressAndAmount> myaddresses;
                        myaddresses = new List<AddressAndAmount>();
                        BitcoinBlockchainController.GetBlockAddressesAndAmounts(ref myaddresses, ref valueIn, ref valueOut, ref fees, ref sblocktime, urlParameters);
                        DateTime dblockdate = UnixTimeStampToDateTime(sblocktime);
                        soutput = soutput + "<font color=orange size=2>Transaction Amount Input: " + valueIn.ToString() + ", Amount Output: " + valueOut.ToString() + ", Fees " + fees.ToString() + ", Date " + dblockdate.ToString() + "</font></br>";
                        foreach (var itemb in myaddresses)
                        {
                            anAddress = itemb;
                            soutput = soutput + "<font color=blue size=1>Address: </font>";
                            soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Wallet ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthetransaction + "&walletid=" + anAddress.myaddress.ToString() +Convert.ToChar(34) + ">";
                            soutput = soutput + "<font color=blue size=1><u>" + anAddress.myaddress.ToString() + "</u></font></a>";
                            soutput = soutput + "<font color=blue size=1>" + ", " + anAddress.inputoutput + " Amount: " + anAddress.myamount.ToString() + "</font></br>";
                        }
                    }
                    else
                    {
                        soutput = "Enter a valid transaction ID";
                    }
                    model.blockchaininfo = "<html><head></head><body><p>" + soutput + "</p></body></html>";

                    return View(model);
                }
                else if (submitButton == "Submit Wallet ID")
                {
                    long ilastblock = 0;
                    long istartblock = 0;
                    blocknumber = Request.QueryString["blocknumber"];

                    lastbitcoinblock = Request.QueryString["lastbitcoinblock"];

                    bool result = false;
                    result = Int64.TryParse(blocknumber, out istartblock);
                    result = Int64.TryParse(lastbitcoinblock, out ilastblock);
                    model.blocknumber = istartblock;
                    model.latestblock = ilastblock;

                    model.blockchaininfo = "";
                    string soutput = "Enter a wallet ID";
                    int inumtransactions = 0;
                    double valueIn = 0;
                    double valueOut = 0;
                    double fees = 0;

                    sthetransaction = Request.QueryString["transactionid"];

                    stheaddress = Request.QueryString["walletid"];

                    model.transactionid = sthetransaction;
                    model.walletid = stheaddress;

                    stheaddress = stheaddress.Trim();
                    urlParameters = addressURL + stheaddress;
                    if (stheaddress != "")
                    {
                        soutput = "<b>Wallet: " + stheaddress + "</b></br>";

                        //(1) Get the address information and transactions
                        double balance = 0;
                        double totalReceived = 0;
                        double totalSent = 0;
                        string sblocktime = "";
                        List<string> mytransactions;
                        mytransactions = new List<string>();
                        BitcoinBlockchainController.GetBlockAddressInfo(ref mytransactions, ref balance, ref totalReceived, ref totalSent, urlParameters);
                        soutput = soutput + "<b>Balance: " + balance.ToString() + "</b></br>";
                        soutput = soutput + "Total Received into Address: " + totalReceived.ToString() + "</br>";
                        soutput = soutput + "Total Sent from Address: " + totalSent.ToString() + "</br>";
                        soutput = soutput + "<b>Number of Transactions: " + mytransactions.Count().ToString() + "</b></br>";
                        //sw.WriteLine(iblockcount.ToString() + "," + sBlockHash + "," + mytransactions.Count().ToString() + "," + dblockdate.ToString());

                        //Get the tramsactions
                        if ((mytransactions != null))
                        {
                            foreach (var item in mytransactions)
                            {
                                inumtransactions++;
                                string sthelocaltransaction = item.ToString();
                                //soutput = soutput + inumtransactions.ToString() + " Transaction ID:" + sthelocaltransaction + "</br>";
                                urlParameters = transactionURL + sthelocaltransaction;
                                List<AddressAndAmount> myaddresses;
                                myaddresses = new List<AddressAndAmount>();
                                BitcoinBlockchainController.GetBlockAddressesAndAmounts(ref myaddresses, ref valueIn, ref valueOut, ref fees, ref sblocktime, urlParameters);
                                DateTime dblockdate = UnixTimeStampToDateTime(sblocktime);
                                soutput = soutput + "<font color=orange size=2>" + inumtransactions.ToString() + ". Transaction Amount Input: " + valueIn.ToString() + ", Amount Output: " + valueOut.ToString() + ", Fees " + fees.ToString() + ", Date " + dblockdate.ToString() + "</font></br>";
                                soutput = soutput + "<font size=2>Transaction ID:</font>";
                                soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Transaction ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthelocaltransaction + "&walletid=" + stheaddress + Convert.ToChar(34) + ">";
                                soutput = soutput + "<font size=2><u>" + sthelocaltransaction + "</u></font></a></br>";
                                foreach (var itemb in myaddresses)
                                {
                                    if (stheaddress == itemb.myaddress)
                                    {
                                        if (itemb.inputoutput == "Input")
                                        {
                                            soutput = soutput + "Sent Amount: " + itemb.myamount.ToString() + "</br>";
                                        }
                                        else if (itemb.inputoutput == "Output")
                                        {
                                            soutput = soutput + "Received Amount: " + itemb.myamount.ToString() + "</br>";
                                        }
                                        else
                                        {
                                            soutput = soutput + "Amount:" + itemb.myamount.ToString() + "</br>";
                                        }
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        soutput = "Please enter a valid wallet ID";
                    } //end of block choice

                    model.blockchaininfo = "<html><head></head><body><p>" + soutput + "</p></body></html>";

                    return View(model);
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                return View(model);
            }

        }

        // POST: BitcoinBlockchain/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection collection, string submitButton)
        {
            try
            {

                if (submitButton == "Submit Block Number")
                {

                    int inumtransactions = 0;
                    long istartblock = 0;
                    long ilastblock = 0;
                    double valueIn = 0;
                    double valueOut = 0;
                    double fees = 0;
                    string blocknumber = collection[1].ToString();
                    if (blocknumber == null) blocknumber = Request.QueryString["blocknumber"];

                    string lastbitcoinblock = collection[2].ToString();
                    if (lastbitcoinblock == null) lastbitcoinblock = Request.QueryString["lastbitcoinblock"];
                    bool result = false;
                    result = Int64.TryParse(blocknumber, out istartblock);
                    result = Int64.TryParse(lastbitcoinblock, out ilastblock);

                    BitcoinBlockchain model = new BitcoinBlockchain();
                    model.blocknumber = istartblock;
                    model.latestblock = ilastblock;
                    model.blockchaininfo = "";
                    string soutput = "Enter a Bitcoin block number within the range 0 and " + ilastblock.ToString();

                    string sthetransaction = collection[3].ToString();
                    if (sthetransaction == null) sthetransaction = Request.QueryString["transactionid"];

                    string stheaddress = collection[4].ToString();
                    if (stheaddress == null) stheaddress = Request.QueryString["walletid"];
                    model.transactionid = sthetransaction;
                    model.walletid = stheaddress;


                    if ((istartblock >= 0) && (istartblock <= ilastblock))
                    {
                        //(1) Get the blockHash
                        string urlParameters = URL + istartblock.ToString();
                        string sBlockHash = "";
                        BitcoinBlockchainController.GetBlockHash(ref sBlockHash, urlParameters);

                        //(2) Get the block transactions
                        //int iblockcount = istartblock;
                        string nextblockHash = "";
                        string sblocktime = "";
                        long blocksize = 0;
                        DateTime dblockdate = System.DateTime.Now;
                        urlParameters = blockURL + sBlockHash;
                        List<string> mytransactions;
                        mytransactions = new List<string>();
                        BitcoinBlockchainController.GetBlockTransactions(ref mytransactions, ref nextblockHash, ref sblocktime, ref blocksize, urlParameters);

                        dblockdate = UnixTimeStampToDateTime(sblocktime);
                        soutput = "<b>Block: " + istartblock.ToString() + " : " + sBlockHash.ToString() + "</br> Time: " + dblockdate.ToString() + "</b> Size: " + blocksize.ToString() + "</br> NumTransactions:" + mytransactions.Count().ToString() + "</br>";
                        //sw.WriteLine(iblockcount.ToString() + "," + sBlockHash + "," + mytransactions.Count().ToString() + "," + dblockdate.ToString());

                        //Get the tramsactions
                        if ((mytransactions != null))
                        {
                            foreach (var item in mytransactions)
                            {
                                inumtransactions++;
                                string sthelocaltransaction = item.ToString();
                                urlParameters = transactionURL + sthelocaltransaction;
                                List<AddressAndAmount> myaddresses;
                                myaddresses = new List<AddressAndAmount>();
                                BitcoinBlockchainController.GetBlockAddressesAndAmounts(ref myaddresses, ref valueIn, ref valueOut, ref fees, ref sblocktime, urlParameters);
                                soutput = soutput + "<font color=orange size=2>" + inumtransactions.ToString() + ". Transaction Amount Input: " + valueIn.ToString() + ", Amount Output: " + valueOut.ToString() + ", Fees " + fees.ToString() + "</font></br>";
                                soutput = soutput + "Transaction ID:";
                                soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Transaction ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthelocaltransaction + "&walletid=" + stheaddress + Convert.ToChar(34) + ">";
                                soutput = soutput + "<u>" + sthelocaltransaction + "</u></a></br>";
                                foreach (var itemb in myaddresses)
                                {
                                    soutput = soutput + "<font color=blue size=1>Address: </font>";
                                    soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Wallet ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthetransaction + "&walletid=" + itemb.myaddress.ToString() + Convert.ToChar(34) + ">";
                                    soutput = soutput + "<font color=blue size=1><u>" + itemb.myaddress.ToString() + "</u></font></a>";
                                    soutput = soutput + "<font color=blue size=1>" + ", " + itemb.inputoutput + " Amount: " + itemb.myamount.ToString() + "</font></br>";
                                }
                                myaddresses.Clear();

                            }
                        }
                    }
                    model.blockchaininfo = "<html><head></head><body><p>" + soutput + "</p></body></html>";

                    return View(model);
                    //return RedirectToAction("Index");
                }
                else if (submitButton == "Submit Transaction ID")
                {
                    BitcoinBlockchain model = new BitcoinBlockchain();
                    long ilastblock = 0;
                    long istartblock = 0;
                    string blocknumber = collection[1].ToString();
                    if (blocknumber == null) blocknumber = Request.QueryString["blocknumber"];

                    string lastbitcoinblock = collection[2].ToString();
                    if (lastbitcoinblock == null) lastbitcoinblock = Request.QueryString["lastbitcoinblock"];

                    bool result = false;
                    result = Int64.TryParse(blocknumber, out istartblock);
                    result = Int64.TryParse(lastbitcoinblock, out ilastblock);
                    model.blocknumber = istartblock;
                    model.latestblock = ilastblock;

                    model.blockchaininfo = "";
                    string soutput = "Enter a transaction ID";
                    double valueIn = 0;
                    double valueOut = 0;
                    double fees = 0;
                    string sblocktime = "";
                    AddressAndAmount anAddress = new AddressAndAmount();

                    string sthetransaction = collection[3].ToString();
                    if (sthetransaction == null) sthetransaction = Request.QueryString["transactionid"];

                    string stheaddress = collection[4].ToString();
                    if (stheaddress == null) stheaddress = Request.QueryString["walletid"];

                    model.transactionid = sthetransaction;
                    model.walletid = stheaddress;

                    if (sthetransaction != "")
                    {
                        soutput = "<b>Transaction: " + sthetransaction + "</b></br>";
                        string urlParameters = transactionURL + sthetransaction;
                        List<AddressAndAmount> myaddresses;
                        myaddresses = new List<AddressAndAmount>();
                        BitcoinBlockchainController.GetBlockAddressesAndAmounts(ref myaddresses, ref valueIn, ref valueOut, ref fees, ref sblocktime, urlParameters);
                        DateTime dblockdate = UnixTimeStampToDateTime(sblocktime);
                        soutput = soutput + "<font color=orange size=2>Transaction Amount Input: " + valueIn.ToString() + ", Amount Output: " + valueOut.ToString() + ", Fees " + fees.ToString() + ", Date " + dblockdate.ToString() +  "</font></br>";
                        foreach (var itemb in myaddresses)
                        {
                            anAddress = itemb;
                            soutput = soutput + "<font color=blue size=1>Address: </font>";
                            soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Wallet ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthetransaction + "&walletid=" + anAddress.myaddress.ToString() +Convert.ToChar(34) + ">";
                            soutput = soutput + "<font color=blue size=1><u>" + anAddress.myaddress.ToString() + "</u></font></a>";
                            soutput = soutput + "<font color=blue size=1>" + ", " + anAddress.inputoutput + " Amount: " + anAddress.myamount.ToString() + "</font></br>";
                        }
                    }
                    else
                    {
                        soutput = "Enter a valid transaction ID";
                    }
                    model.blockchaininfo = "<html><head></head><body><p>" + soutput + "</p></body></html>";

                    return View(model);
                }
                else if (submitButton == "Submit Wallet ID")
                {
                    BitcoinBlockchain model = new BitcoinBlockchain();
                    long ilastblock = 0;
                    long istartblock = 0;
                    string blocknumber = collection[1].ToString();
                    if (blocknumber == null) blocknumber = Request.QueryString["blocknumber"];

                    string lastbitcoinblock = collection[2].ToString();
                    if (lastbitcoinblock == null) lastbitcoinblock = Request.QueryString["lastbitcoinblock"];

                    bool result = false;
                    result = Int64.TryParse(blocknumber, out istartblock);
                    result = Int64.TryParse(lastbitcoinblock, out ilastblock);
                    model.blocknumber = istartblock;
                    model.latestblock = ilastblock;

                    model.blockchaininfo = "";
                    string soutput = "Enter a wallet ID";
                    int inumtransactions = 0;
                    double valueIn = 0;
                    double valueOut = 0;
                    double fees = 0;
                    string sblocktime = "";

                    string sthetransaction = collection[3].ToString();
                    if (sthetransaction == null) sthetransaction = Request.QueryString["transactionid"];

                    string stheaddress = collection[4].ToString();
                    if (stheaddress == null) stheaddress = Request.QueryString["walletid"];

                    model.transactionid = sthetransaction;
                    model.walletid = stheaddress;

                    stheaddress = stheaddress.Trim();
                    string urlParameters = addressURL + stheaddress;
                    if (stheaddress != "")
                    {
                        soutput = "<b>Wallet: " + stheaddress + "</b></br>";

                        //(1) Get the address information and transactions
                        double balance = 0;
                        double totalReceived = 0;
                        double totalSent = 0;
                        List<string> mytransactions;
                        mytransactions = new List<string>();
                        BitcoinBlockchainController.GetBlockAddressInfo(ref mytransactions, ref balance, ref totalReceived, ref totalSent, urlParameters);
                        soutput = soutput + "<b>Balance: " + balance.ToString() + "</b></br>";
                        soutput = soutput + "Total Received into Address: " + totalReceived.ToString() + "</br>";
                        soutput = soutput + "Total Sent from Address: " + totalSent.ToString() + "</br>";
                        soutput = soutput + "<b>Number of Transactions: " + mytransactions.Count().ToString() + "</b></br>";
                        //sw.WriteLine(iblockcount.ToString() + "," + sBlockHash + "," + mytransactions.Count().ToString() + "," + dblockdate.ToString());

                        //Get the tramsactions
                        if ((mytransactions != null))
                        {
                            foreach (var item in mytransactions)
                            {
                                inumtransactions++;
                                string sthelocaltransaction = item.ToString();
                                //soutput = soutput + inumtransactions.ToString() + " Transaction ID:" + sthelocaltransaction + "</br>";
                                urlParameters = transactionURL + sthelocaltransaction;
                                List<AddressAndAmount> myaddresses;
                                myaddresses = new List<AddressAndAmount>();
                                BitcoinBlockchainController.GetBlockAddressesAndAmounts(ref myaddresses, ref valueIn, ref valueOut, ref fees, ref sblocktime, urlParameters);
                                DateTime dblockdate = UnixTimeStampToDateTime(sblocktime);
                                soutput = soutput + "<font color=orange size=2>" + inumtransactions.ToString() + ". Transaction Amount Input: " + valueIn.ToString() + ", Amount Output: " + valueOut.ToString() + ", Fees " + fees.ToString() + ", Date " + dblockdate.ToString() + "</font></br>";
                                soutput = soutput + "<font size=2>Transaction ID:</font>";
                                soutput = soutput + "<a href=" + Convert.ToChar(34) + "/BitcoinBlockchain/?submitButton=Submit Transaction ID&blocknumber=" + blocknumber + "&lastbitcoinblock=" + lastbitcoinblock + "&transactionid=" + sthelocaltransaction + "&walletid=" + stheaddress + Convert.ToChar(34) + ">";
                                soutput = soutput + "<font size=2><u>" + sthelocaltransaction + "</u></font></a></br>";
                                foreach (var itemb in myaddresses)
                                {
                                    if (stheaddress == itemb.myaddress)
                                    {
                                        if (itemb.inputoutput == "Input")
                                        {
                                            soutput = soutput + "Sent Amount: " + itemb.myamount.ToString() + "</br>";
                                        }
                                        else if (itemb.inputoutput == "Output")
                                        {
                                            soutput = soutput + "Received Amount: " + itemb.myamount.ToString() + "</br>";
                                        }
                                        else
                                        {
                                            soutput = soutput + "Amount:" + itemb.myamount.ToString() + "</br>";
                                        }
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        soutput = "Please enter a valid wallet ID";
                    } //end of block choice

                    model.blockchaininfo = "<html><head></head><body><p>" + soutput + "</p></body></html>";

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }
        
    }
}
