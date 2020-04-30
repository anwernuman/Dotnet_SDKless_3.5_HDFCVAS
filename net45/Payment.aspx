<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="RazorpaySampleApp.Payment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Razorpay .Net Sample App</title>
</head>
<body>
<button id="rzp-button1">Pay</button>
<script src="https://checkout.razorpay.com/v1/checkout.js"></script>
<script>
    var options = {
        "key": "API_KEY",
        "amount": "100",
        "name": "Checkout Test",
        "description": "Any Description",
        "order_id":"<%=orderId%>",
        "image": "http://80.211.43.229/60cef340-345d-4258-be84-f69ca9271dfb.gif",
        //"modal":{ ondismiss:function(){ alert('Popup closed') } }, //control comes here after the popup is closed
        //"timeout":"100", //100 seconds
        "redirect":"true",
        "callback_url": "http://127.0.0.1:8080/Charge.aspx",
        "prefill": {
            "name": "Gaurav Kumar",
            "email": "test@test.com",
            "contact": "9876543210"
        },
        "notes": {
            "reference_no": "ABCD123" //your reference number
        },
        "theme": {
            "color": "#000000"
        }
    };
    var rzp1 = new Razorpay(options);
    document.getElementById('rzp-button1').onclick = function (e) {
        rzp1.open();
        e.preventDefault();
    }
    </script>
</body>
</html>
