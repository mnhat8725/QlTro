// hopdong.js
// Xử lý copy và in thông tin tài khoản người thuê

// Copy thông tin tài khoản
function copyAccountInfo() {
    var alertDiv = document.querySelector('.alert-info');
    if (alertDiv) {
        var text = alertDiv.innerText;
        navigator.clipboard.writeText(text).then(function () {
            alert('✅ Đã copy thông tin tài khoản!\n\nBạn có thể paste để gửi cho người thuê.');
        }, function (err) {
            console.error('Lỗi khi copy:', err);
            alert('❌ Không thể copy. Vui lòng copy thủ công.');
        });
    }
}

// In phiếu thông tin
function printAccountInfo() {
    var alertDiv = document.querySelector('.alert-info');
    if (alertDiv) {
        var content = alertDiv.innerHTML;
        var printWindow = window.open('', '', 'height=600,width=800');
        printWindow.document.write('<html><head><title>Thông tin tài khoản đăng nhập</title>');
        printWindow.document.write('<style>');
        printWindow.document.write('body { font-family: Arial, sans-serif; padding: 40px; }');
        printWindow.document.write('.header { text-align: center; border-bottom: 3px solid #333; padding-bottom: 20px; margin-bottom: 30px; }');
        printWindow.document.write('.content { font-size: 16px; line-height: 2; }');
        printWindow.document.write('.footer { margin-top: 50px; text-align: center; font-size: 12px; color: #666; }');
        printWindow.document.write('.box { border: 2px solid #333; padding: 20px; margin: 20px 0; }');
        printWindow.document.write('</style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write('<div class="header"><h1>NHÀ TRỌ NGÃO GIÁ</h1><h2>THÔNG TIN TÀI KHOẢN ĐĂNG NHẬP</h2></div>');
        printWindow.document.write('<div class="box content">' + content + '</div>');
        printWindow.document.write('<div class="content"><p><strong>Hướng dẫn đăng nhập:</strong></p><ol><li>Truy cập website: <strong>' + window.location.origin + '</strong></li><li>Click "Đăng nhập"</li><li>Nhập tài khoản và mật khẩu ở trên</li><li>Sau khi đăng nhập, bạn có thể đổi mật khẩu</li></ol></div>');
        printWindow.document.write('<div class="footer"><p>Vui lòng giữ thông tin này bảo mật</p><p>Ngày in: ' + new Date().toLocaleDateString('vi-VN') + '</p></div>');
        printWindow.document.write('</body></html>');
        printWindow.document.close();

        setTimeout(function () {
            printWindow.print();
        }, 500);
    }
}
