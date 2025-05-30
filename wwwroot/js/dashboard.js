function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    sidebar.classList.toggle("collapsed");
}

// --- USER ROLE CONFIG ---
const userRolesConfig = {
    "rd-staff": {
        name: "R&D Staff",
        avatarInitials: "RM",
        sidebarMenu: [
            { name: "Dashboard", icon: "fas fa-tachometer-alt", link: "#" },
            { name: "Subject Management", icon: "fas fa-book", link: "#subjects" },
            { name: "Question Bank", icon: "fas fa-database", link: "#questions" },
            { name: "Duplication Check", icon: "fas fa-search-plus", link: "#duplicates" },
            { name: "Plans & Distribution", icon: "fas fa-tasks", link: "#plans" },
            { name: "Exam Management", icon: "fas fa-file-alt", link: "#exams" },
            { name: "Reports", icon: "fas fa-chart-bar", link: "#reports-rd" },
            { name: "Settings", icon: "fas fa-cog", link: "#settings" }
        ],
        dashboardHtml: `<div class='welcome-banner'><h1>Welcome, R&D!</h1><p>Manage questions & duplication process.</p></div>`
    },
    "lecturer": {
        name: "Lecturer",
        avatarInitials: "LE",
        sidebarMenu: [
            { name: "Dashboard", icon: "fas fa-tachometer-alt", link: "#" },
            { name: "My Tasks", icon: "fas fa-clipboard-list", link: "#tasks" },
            { name: "Upload Questions", icon: "fas fa-upload", link: "#upload" },
            { name: "Check Duplicates", icon: "fas fa-copy", link: "#check-duplicates" },
            { name: "Mock Exams", icon: "fas fa-file-signature", link: "#mock-exams" }
        ],
        dashboardHtml: `<div class='welcome-banner'><h1>Hello, Lecturer!</h1><p>Your current tasks are listed below.</p></div>`
    },
    "head-dept": {
        name: "Head of Department",
        avatarInitials: "HD",
        sidebarMenu: [
            { name: "Dashboard", icon: "fas fa-tachometer-alt", link: "#" },
            { name: "Department Overview", icon: "fas fa-building", link: "#overview" },
            { name: "Approve Plans", icon: "fas fa-check-square", link: "#approve" }
        ],
        dashboardHtml: `<div class='welcome-banner'><h1>Welcome, Head of Department!</h1><p>Monitor department-wide progress and approvals.</p></div>`
    },
    "subject-leader": {
        name: "Subject Leader",
        avatarInitials: "SL",
        sidebarMenu: [
            { name: "Dashboard", icon: "fas fa-tachometer-alt", link: "#" },
            { name: "Team Tasks", icon: "fas fa-users", link: "#team-tasks" },
            { name: "Approve Submissions", icon: "fas fa-clipboard-check", link: "#submissions" }
        ],
        dashboardHtml: `<div class='welcome-banner'><h1>Hello, Subject Leader!</h1><p>Manage and approve tasks from your team.</p></div>`
    },
    "exam-head": {
        name: "Head of Examination Department",
        avatarInitials: "EH",
        sidebarMenu: [
            { name: "Dashboard", icon: "fas fa-tachometer-alt", link: "#" },
            { name: "Exam Schedule", icon: "fas fa-calendar-alt", link: "#schedule" },
            { name: "Final Approval", icon: "fas fa-check-circle", link: "#final-approval" }
        ],
        dashboardHtml: `<div class='welcome-banner'><h1>Greetings, Examination Head!</h1><p>Oversee final exam coordination and approvals.</p></div>`
    }
};

function loadSidebarByRole(roleKey) {
    const role = userRolesConfig[roleKey];
    const sidebar = document.getElementById("sidebarMenu");
    if (!role || !sidebar) return;

    sidebar.innerHTML = "";
    role.sidebarMenu.forEach(item => {
        const li = document.createElement("li");
        li.innerHTML = `
            <a href="${item.link}" class="${item.active ? 'active' : ''}">
                <i class="${item.icon} menu-icon"></i>
                <span class="menu-text">${item.name}</span>
            </a>`;
        sidebar.appendChild(li);
    });

    // Cập nhật avatar + tên + chức danh
    document.getElementById("userAvatar").innerText = role.avatarInitials;
    document.getElementById("profileUserName").innerText = role.name;
    document.getElementById("profileUserRole").innerText = role.name;
}

function loadDashboardHtml(roleKey) {
    const role = userRolesConfig[roleKey];
    const container = document.getElementById("dashboardContent") || document.querySelector("main.dashboard-container");
    if (!role || !container) return;
    container.innerHTML = role.dashboardHtml;
}

// Avatar dropdown menu toggle
document.addEventListener("DOMContentLoaded", function () {
    const trigger = document.getElementById("userProfileTrigger");
    const dropdown = document.getElementById("userDropdownContent");

    if (trigger && dropdown) {
        trigger.addEventListener("click", function (e) {
            e.stopPropagation();
            dropdown.classList.toggle("active");
        });

        document.addEventListener("click", function (e) {
            if (!trigger.contains(e.target) && !dropdown.contains(e.target)) {
                dropdown.classList.remove("active");
            }
        });
    }
});
