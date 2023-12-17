export default {
    defaultTheme: 'light',
    showLightbox: (img) => true,
    iconLinks: [
        {
            icon: 'github',
            href: 'https://github.com/sator-imaging',
            title: 'GitHub'
        },
        {
            icon: 'twitter',
            href: 'https://twitter.com/sator_imaging',
            title: 'Twitter'
        },
        {
            icon: 'youtube',
            href: 'https://www.youtube.com/@SatorImaging',
            title: 'YouTube'
        },
        {
            icon: 'chat-quote-fill',
            href: 'https://www.sator-imaging.com/',
            title: 'Contact'
        },
    ],
}


// badge for api heading
function addApiHeadingBadge(event) {
    let apiTitle = document.querySelector("h1.api");
    if (apiTitle) {
        let badgeText = undefined;

        if (apiTitle.dataset?.commentid?.at(1) == ':') {
            let pos = apiTitle.innerText.indexOf(' ');
            if (pos >= 0)
                badgeText = apiTitle.innerText.slice(0, pos);
        }

        if (badgeText) {
            let badge = document.createElement('span');
            badge.innerText = badgeText;
            badge.classList.add("badge");
            badge.classList.add("bg-info");
            badge.classList.add("badge-api");
            apiTitle.innerText = apiTitle.innerText.replace(badgeText, '') + ' ';
            //apiTitle.parentNode.insertBefore(badge, apiTitle);
            apiTitle.appendChild(badge);
        }
    }
}

if (document.readyState == 'loading') {
    window.addEventListener("DOMContentLoaded", ev => addApiHeadingBadge(ev));
} else {
    addApiHeadingBadge(undefined);
}
