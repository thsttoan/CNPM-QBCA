// Sidebar toggle
// function toggleSidebar() {
//    var sidebar = document.getElementById("sidebar");
//    if (sidebar.classList.contains("collapsed")) {
//        sidebar.classList.remove("collapsed");
//        document.querySelector(".main-content").style.marginLeft = "220px";
//    } else {
//        sidebar.classList.add("collapsed");
//        document.querySelector(".main-content").style.marginLeft = "60px";
//    }
//} 

// User avatar dropdown
document.addEventListener("DOMContentLoaded", function () {
    var trigger = document.getElementById("userProfileTrigger");
    var dropdown = document.getElementById("userDropdownContent");
    if (trigger && dropdown) {
        trigger.addEventListener("click", function (e) {
            e.stopPropagation();
            dropdown.classList.toggle("show");
        });
        document.addEventListener("click", function (e) {
            if (!dropdown.contains(e.target) && !trigger.contains(e.target)) {
                dropdown.classList.remove("show");
            }
        });
    }

    // Notification bell (optional demo)
    var bell = document.querySelector('.notification-bell .icon-wrapper');
    var notiDrop = document.querySelector('.notification-bell');
    if (bell && notiDrop) {
        bell.addEventListener('click', function (e) {
            e.stopPropagation();
            notiDrop.classList.toggle('open');
        });
        document.addEventListener('click', function (e) {
            if (!notiDrop.contains(e.target)) {
                notiDrop.classList.remove('open');
            }
        });
    }
});