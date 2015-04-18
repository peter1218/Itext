using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDFDocumentGeneration.Models;
namespace PDFDocumentGeneration.Repository
{
    public static class FakeRepository
    {



        public static List<AccountOptionsModelcs> accountRepaymentList()
        {
            List<AccountOptionsModelcs> accountOptionsList = new List<AccountOptionsModelcs>();
            AccountOptionsModelcs manager1 = new AccountOptionsModelcs();
            manager1.loanAgreementType = LoanAgreementType.Basic;
            manager1.AccountDetail = " Variable rate account with flexible repayment option";
            manager1.AccountType = "Account A" ;
            manager1.money = 150;
            AccountOptionsModelcs manager2 = new AccountOptionsModelcs();
            manager2.loanAgreementType = LoanAgreementType.Basic;
            manager2.AccountDetail = " Fixed rate account";
            manager2.AccountType = "Account B" ;
            manager2.money = 150;

            AccountOptionsModelcs manager3 = new AccountOptionsModelcs();
            manager3.loanAgreementType = LoanAgreementType.Basic;
            manager3.AccountDetail = " Line of credit account";
            manager3.AccountType = "Account C";
            manager3.money = 150;

            AccountOptionsModelcs manager4 = new AccountOptionsModelcs();
            manager4.loanAgreementType = LoanAgreementType.Basic;
            manager4.AccountDetail = " Construction loan account";
            manager4.AccountType = "Account D" ;
            manager4.money = 150;

            AccountOptionsModelcs manager5 = new AccountOptionsModelcs();
            manager5.loanAgreementType = LoanAgreementType.Basic;
            manager5.AccountDetail = " Transitional loan account";
            manager5.AccountType = "Account E";
            manager5.money = 150;



            accountOptionsList.Add(manager1);
            accountOptionsList.Add(manager2);
            accountOptionsList.Add(manager3);
            accountOptionsList.Add(manager4);
            accountOptionsList.Add(manager5);

            return accountOptionsList;
        }


       

        public static List<AccountOptionsModelcs> accountList()
        {
            List<AccountOptionsModelcs> accountOptionsList = new List<AccountOptionsModelcs>();
            AccountOptionsModelcs manager1 = new AccountOptionsModelcs();
            manager1.loanAgreementType = LoanAgreementType.Basic;
            manager1.AccountDetail = " Variable rate account with flexible repayment option";
            manager1.AccountType = "Account A:     " + manager1.loanAgreementType.ToString() + manager1.AccountDetail ;
            manager1.money = 150;
            AccountOptionsModelcs manager2 = new AccountOptionsModelcs();
            manager2.loanAgreementType = LoanAgreementType.Basic;
            manager2.AccountDetail = " Variable rate account with flexible repayment option";
            manager2.AccountType = "Account A:     " + manager1.loanAgreementType.ToString() + manager1.AccountDetail;
            manager2.money = 150;

            AccountOptionsModelcs manager3 = new AccountOptionsModelcs();
            manager3.loanAgreementType = LoanAgreementType.Basic;
            manager3.AccountDetail = " Variable rate account with flexible repayment option";
            manager3.AccountType = "Account A:     " + manager1.loanAgreementType.ToString() + manager1.AccountDetail;
            manager3.money = 150;

            AccountOptionsModelcs manager4 = new AccountOptionsModelcs();
            manager4.loanAgreementType = LoanAgreementType.Basic;
            manager4.AccountDetail = " Variable rate account with flexible repayment option";
            manager4.AccountType = "Account A:     " + manager1.loanAgreementType.ToString() + manager1.AccountDetail;
            manager4.money = 150;

            accountOptionsList.Add(manager1);
            accountOptionsList.Add(manager2);
            accountOptionsList.Add(manager4);
            accountOptionsList.Add(manager3);
         

            return accountOptionsList;
        }

    }
}