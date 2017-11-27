///* Thai initialisation for the jQuery UI date picker plugin. */
///* Written by pipo (pipo@sixhead.com). */
//jQuery(function($){
//    $.datepicker.regional['th'] = {
//        { dateFormat: 'mm/dd/yy', isBuddhist: true, defaultDate: toDay, dayNames: ['อาทิตย์', 'จันทร์', 'อังคาร', 'พุธ', 'พฤหัสบดี', 'ศุกร์', 'เสาร์'],
//    	    dayNamesMin: ['อา.', 'จ.', 'อ.', 'พ.', 'พฤ.', 'ศ.', 'ส.'],
//    	    monthNames: ['มกราคม', 'กุมภาพันธ์', 'มีนาคม', 'เมษายน', 'พฤษภาคม', 'มิถุนายน', 'กรกฎาคม', 'สิงหาคม', 'กันยายน', 'ตุลาคม', 'พฤศจิกายน', 'ธันวาคม'],
//    	    monthNamesShort: ['ม.ค.', 'ก.พ.', 'มี.ค.', 'เม.ย.', 'พ.ค.', 'มิ.ย.', 'ก.ค.', 'ส.ค.', 'ก.ย.', 'ต.ค.', 'พ.ย.', 'ธ.ค.']
//    	}
//    };
//	$.datepicker.setDefaults($.datepicker.regional['th']);
//});


/* Thai initialisation for the jQuery UI date picker plugin. */
/* Written by pipo (pipo@sixhead.com). */
jQuery(function ($) {
    var d = new Date();
    var toDay = (d.getMonth() + 1) + '/' + d.getDate() + '/' + (d.getFullYear() + 543);
//    var toDay = d.getDate() + '/'
//        + (d.getMonth() + 1) + '/'
//        + (d.getFullYear() + 543);
    
    $.datepicker.regional['th'] = {
        defaultDate: toDay,
        changeMonth: true,
        changeYear: true,
        isBuddhist: true,
        closeText: 'ปิด',
        prevText: '&laquo;&nbsp;ย้อน',
        nextText: 'ถัดไป&nbsp;&raquo;',
        currentText: 'วันนี้',
        monthNames: ['มกราคม', 'กุมภาพันธ์', 'มีนาคม', 'เมษายน', 'พฤษภาคม', 'มิถุนายน', 'กรกฏาคม', 'สิงหาคม', 'กันยายน', 'ตุลาคม', 'พฤศจิกายน', 'ธันวาคม'],
        monthNamesShort: ['ม.ค.', 'ก.พ.', 'มี.ค.', 'เม.ย.', 'พ.ค.', 'มิ.ย.', 'ก.ค.', 'ส.ค.', 'ก.ย.', 'ต.ค.', 'พ.ย.', 'ธ.ค.'],
        dayNames: ['อาทิตย์', 'จันทร์', 'อังคาร', 'พุธ', 'พฤหัสบดี', 'ศุกร์', 'เสาร์'],
        dayNamesShort: ['อา.', 'จ.', 'อ.', 'พ.', 'พฤ.', 'ศ.', 'ส.'],
        dayNamesMin: ['อา.', 'จ.', 'อ.', 'พ.', 'พฤ.', 'ศ.', 'ส.'],
        dateFormat: 'mm/dd/yy', firstDay: 0,
        isBE: true
    };
    $.datepicker.setDefaults($.datepicker.regional['th']);
});

