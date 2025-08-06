$(document).ready(function () {
    const themeToggle = $('#theme-toggle');
    const body = $('body');
    const moonIcon = 'fa-moon';
    const sunIcon = 'fa-sun';

    function applyTheme() {
        if (localStorage.getItem('theme') === 'dark') {
            body.addClass('dark-mode');
            themeToggle.find('i').removeClass(moonIcon).addClass(sunIcon);
        } else {
            body.removeClass('dark-mode');
            themeToggle.find('i').removeClass(sunIcon).addClass(moonIcon);
        }
    }

    applyTheme();

    themeToggle.on('click', function (e) {
        e.preventDefault();

        body.toggleClass('dark-mode');

        if (body.hasClass('dark-mode')) {
            localStorage.setItem('theme', 'dark');
            themeToggle.find('i').removeClass(moonIcon).addClass(sunIcon);
        } else {
            localStorage.setItem('theme', 'light');
            themeToggle.find('i').removeClass(sunIcon).addClass(moonIcon);
        }
    });
});