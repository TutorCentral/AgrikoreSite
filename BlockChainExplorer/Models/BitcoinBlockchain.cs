using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace BlockChainExplorer.Models
{
    public class BitcoinBlockchain
    {
        int selectedtask { get;  set;}

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Enter block number")]
        public long blocknumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Enter start block number")]
        public long startblock { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Enter end block number")]
        public long endblock { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Enter trxn. ID")]
        public string transactionid { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Enter wallet ID")]
        public string walletid { get; set; }

        public string blockchaininfo { get; set; }
        public long latestblock { get; set; }

    }
}