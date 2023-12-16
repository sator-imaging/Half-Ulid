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
        let typeName = undefined;
        if (apiTitle.dataset.commentid.startsWith("N:")) {
            typeName = "Namespace";
        }
        else if (apiTitle.dataset.commentid.startsWith("T:")) {
            typeName = "Class";
        }

        if (typeName) {
            let badge = document.createElement('span');
            badge.innerText = typeName;
            badge.style.fontSize = '1rem';
            badge.style.lineHeight = '1em';
            badge.style.fontWeight = 200;
            badge.style.letterSpacing = '0px';
            badge.style.verticalAlign = 'super';
            badge.classList.add("badge");
            badge.classList.add("bg-info");
            apiTitle.innerText = apiTitle.innerText.replace(typeName, '') + ' ';
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
