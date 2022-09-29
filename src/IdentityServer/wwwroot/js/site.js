// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

function check_all_required() {
    const $f = $('#credsform');
    const $b = $f.find('button');
    const $r = $f.find('[required]');
    let enabled = true;
    $r.each(function() {
        enabled &= !!$(this).val().length;
    });
    $b.prop('disabled', !enabled);
}

function tick() {
    const $this = $(this);
    const ticks = $this.text();
    if (ticks > 0) {
        $this.text(ticks - 1);
        $this.parent().removeClass('d-none');
        setTimeout(function() {
            $this.trigger('tick')
        }, 1000);
    } else {
        $this.parent().addClass('d-none');
    }
}

$(window).on('load', function() {

    const $t = $('.ticker')
    if ($t.length)
        $t.on('tick', tick).trigger('tick');

    const $f = $('#credsform');
    if ($f.length) {
        $f.find('[required]').on('input', check_all_required);
    }
});
