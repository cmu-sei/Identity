// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

function ticker($div, ticks, toggleClass) {
    this.reset = function(v) {
        console.log(ticks);
        let inactive = !ticks;
        ticks = v;
        if (inactive) $div.trigger('tick');
    }
    function tick() {
        let $this = $(this),
            ticks = $this.data('ticks');

        if (ticks) {
            $this.find('.ticker').text(ticks);
            $this.data('ticks', ticks-1).removeClass(toggleClass);
            setTimeout(function() {
                $this.trigger('tick')
            }, 1000);
        } else {
            $this.addClass(toggleClass);
        }
    }

    $div.data('ticks', ticks).on('tick', tick).trigger('tick');
}

function enableForm($div) {
    function check() {
        let enabled = true;
        $r.each(function() {
            enabled &= $(this).val().length;
        });
        $b.prop('disabled', !enabled);
    }
    const $b = $div.find("button");
    const $r = $div.find("[required]");
    $r.on('input', check);
}
