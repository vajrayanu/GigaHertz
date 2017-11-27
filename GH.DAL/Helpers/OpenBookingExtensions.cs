using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GH.DAL.Helpers
{
    public enum UserRole
    {
        Admin=1,
        SuperUser=2,
        User=3
    }

    public enum WorkingType
    {
        repair=0,
        claim=1
    }
    
    public enum Working
    {
        //Default=0,
        //Open=1,
        //Repair=2,
        //Claim=3,
        //Remind=4,
        //Back=5,
        //Close=6,
        //Cancle=7,
        //QC=8,
        //RepairCheck=9,
        //ClaimRecieved=10,
        //Claiming=11,
        //RepairSuccess=12,
        //RemindCause=13

        Default = 0,
        Open = 1,
        OpenBack = 2,
        Repair =3,
        RepairSuccess = 4,
        Claim = 5,
        Claiming =6,
        ClaimRecieved =7,
        RemindCause =8,
        ConfirmRepair=9,
        Cancle=10,
        Remind=11,
        QC=12,
        RepairCheck=13,
        Close=14

        /*  0 มอบหมายงาน
            1 เปิด job
            2 เปิด job (ตีกลับ)
            3 ซ่อม
            4 ซ่อมเสร็จ
            5 เครม
            6 ส่งเครม
            7 รับคืนสินค้าส่งเครม
            8 แจ้งลูกค้าก่อนซ่อม
            9 ดำเนินการซ่อม
            10 ยกเลิกการซ่อม
            11 แจ้งลูกค้ารับสินค้า
            12 ตรวจสอบคุณภาพ
            13 ตรวจสอบผลงานช่าง
            14 ปิด job*/
    }

    public enum Close
    {
        Normal = 1,
        Back = 2,
        Cancle = 3,
        IsInsurance = 4,
        Free = 5,
        NoFree = 6,
        HpOnSite = 7
    }


    public enum CharBooking
    {
        SV=1,
        E=2,
        C=3
    }
}
