$(function () {
    function run_waitMe(effect) {
        $('body').waitMe({
            effect: effect,
            text: 'Please wait...',
            bg: 'rgba(0,0,0,0.5)',
            color: '#000',
            maxSize: '',
            sourcer: 'img.svg',
            onClose: function () { }

        });
    }

    //form validation
    $('#applicationSBT').click(FormValidation);

    function FormValidation() {
        toastr.options = {
            closeButton: false,
            debug: false,
            newestOnTop: true,
            progressBar: false,
            positionClass: 'toast-top-right',
            preventDuplicates: false,
            onclick: null
        };
        var imgVal = $("#userimage").attr("src");

        if (imgVal === "/images/avatar.png") {
            toastr.error('capture image');
        } else 
        if ($('#Department').val() === "") {
            toastr.error('Select Department');
        } else if ($('#TitleCode').val() === "") {
            toastr.error('Enter Title');
        } else if ($('#FirstName').val() === "") {
            toastr.error('Enter FirstName');
        } else if ($('#LastName').val() === "") {
            toastr.error('Enter FirstName');
        } else if ($('#Sex').val() === "") {
            toastr.error('Enter Sex');
        } else if ($('#MaritalStatus').val() === "") {
            toastr.error('Enter Marital Status');
        } else if ($('#OfficePhoneNo').val() === "") {
            toastr.error('Enter Office Phone No');
        } else if ($('#GSMNo').val() === "") {
            toastr.error('Enter GSM No');
        }
        //else if (!isEmail($('#EmailAddress').val().trim()) && $('#EmailAddress').val() !== "") {
        //    toastr.error('Valid Email is required');
        //}
        else if ($('#NameonCard').val() === "") {
            toastr.error('Enter Name on Card');
        }
        else if (!IsSpacialXter($('#NameonCard').val())) {
            toastr.error('Special character not allowed for Name of Card');
        } else if ($.trim($('#NameonCard').val()).length >= 21) {
            toastr.error('Card name cannot be more that 21 characters');
        }
        else if ($('#IDCardType').val() === "") {
            toastr.error('Enter  ID Card Type');
        } else if ($('#IDNo').val() === "") {
            toastr.error('Enter ID No');
        } else if ($('#IDIssueDate').val() === "") {
            toastr.error('Enter ID Issue Date');
        } else if ($('#IDExpiryDate').val() === "") {
            toastr.error('Enter ID Expiry Date');
        } else if ($('#SocioProfCode').val() === "") {
            toastr.error('Enter Socio Prof Code');
        } else if ($('#DateofBirth').val() === "") {
            toastr.error('Enter Date of Birth');
        } else if ($('#Nationality').val() === "") {
            toastr.error('Enter Nationality');
        } else if ($('#RequestingBanchCode').val() === "") {
            toastr.error('Enter Requesting Banch Code');
        } else if ($('#State').val() === "") {
            toastr.error('Enter State');
        } else if ($('#City').val() === "") {
            toastr.error('Enter City');
        } else if ($('#OfficeAddress1').val() === "") {
            toastr.error('Enter Office Address 1');
        } else if ($('#OfficeAddress1').val() === "") {
            toastr.error('Enter Office Address 1');
        } else if ($('#OfficeAddress2').val() === "") {
            toastr.error('Enter Office Address 2');
        } else {
            //run_waitMe('roundBounce');
            if (confirm('Are you sure you want to submit')) {

                $('#applicationBTN').click();
                //run_waitMe('roundBounce');

            }
        }


    }

    function isEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }

    function IsSpacialXter(str) {
        console.log($.trim(str).length);
        return /^[a-zA-Z\s]+$/.test(str)
        //  /^[a-zA-Z0-9]*$/
    }


});