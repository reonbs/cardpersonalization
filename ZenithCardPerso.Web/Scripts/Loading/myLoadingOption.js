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
});