// notifications.js
document.addEventListener('DOMContentLoaded', () => {
    const notifList = document.getElementById('notification-list');
    const badge = document.getElementById('notification-badge');
    const markAllBtn = document.getElementById('mark-all-read-btn');
    
    // Notification sound
    const notificationSound = new Audio('https://assets.mixkit.co/active_storage/sfx/2574/2574-preview.mp3');

    // Make updateBadgeCount global so the partial view can call it
    window.updateBadgeCount = function(count) {
        if (badge) {
            if (count > 0) {
                badge.innerText = count > 99 ? '99+' : count;
                badge.classList.remove('hidden');
            } else {
                badge.innerText = '0';
                badge.classList.add('hidden');
            }
        }
    };

    function loadNotifications() {
        if (!notifList) return;
        fetch('/Notification/Recent')
            .then(res => res.text())
            .then(html => {
                notifList.innerHTML = html;
                const countData = notifList.querySelector('#unread-count-data');
                if (countData) {
                    const count = parseInt(countData.getAttribute('data-count')) || 0;
                    window.updateBadgeCount(count);
                }
                attachClickEvents();
            })
            .catch(err => console.error('Error fetching notifications:', err));
    }

    if (notifList && badge) {
        loadNotifications();
    }

    // Mark as read click event
    function attachClickEvents() {
        document.querySelectorAll('.notification-item').forEach(item => {
            // Remove existing listener to prevent duplicates
            const newItem = item.cloneNode(true);
            item.parentNode.replaceChild(newItem, item);
            
            newItem.addEventListener('click', function() {
                const id = this.getAttribute('data-id');
                const url = this.getAttribute('data-url');
                const unreadDot = this.querySelector('.unread-dot');
                
                if (unreadDot) {
                    fetch(`/Notification/MarkAsRead/${id}`, { method: 'POST' })
                        .then(res => {
                            if(res.ok) {
                                unreadDot.remove();
                                this.classList.add('opacity-70');
                                let currentCount = parseInt(badge.innerText);
                                if (currentCount > 0) window.updateBadgeCount(currentCount - 1);
                            }
                            if (url && url !== '#') {
                                window.location.href = url;
                            }
                        });
                } else {
                    if (url && url !== '#') {
                        window.location.href = url;
                    }
                }
            });
        });
    }

    // Mark all as read
    if (markAllBtn) {
        markAllBtn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            fetch('/Notification/MarkAllAsRead', { method: 'POST' })
                .then(res => {
                    if(res.ok) {
                        window.updateBadgeCount(0);
                        document.querySelectorAll('.unread-dot').forEach(d => d.remove());
                        document.querySelectorAll('.notification-item').forEach(i => i.classList.add('opacity-70'));
                    }
                });
        });
    }

    // Get target URL based on user role and reference type (For Toastr notification click)
    function getNotificationUrl(notif) {
        if (!notif.notificationId) return "#";
        const isEmployer = window.DevHub && window.DevHub.isEmployer;
        return isEmployer 
            ? `/recruiter/notifications/details/${notif.notificationId}` 
            : `/candidate/notifications/details/${notif.notificationId}`;
    }

    // SignalR Setup
    if (typeof signalR !== 'undefined') {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveNotification", function (notification) {
            // Re-fetch HTML partial from server to update the dropdown list
            loadNotifications();

            // Play sound
            notificationSound.play().catch(e => console.log('Audio play failed', e));

            // Show toast
            if (typeof toastr !== 'undefined') {
                toastr.options = {
                    "closeButton": true,
                    "progressBar": true,
                    "positionClass": "toast-bottom-right",
                    "timeOut": "5000",
                    "onclick": function() {
                        const targetUrl = getNotificationUrl(notification);
                        if (targetUrl && targetUrl !== '#') {
                            window.location.href = targetUrl;
                        }
                    }
                };
                
                const title = notification.title || "Thông báo mới";
                const message = notification.message || "";
                
                if (notification.severityLevel === 'success') {
                    toastr.success(message, title);
                } else if (notification.severityLevel === 'warning') {
                    toastr.warning(message, title);
                } else if (notification.severityLevel === 'danger') {
                    toastr.error(message, title);
                } else {
                    toastr.info(message, title);
                }
            }
        });

        connection.start().catch(function (err) {
            return console.error(err.toString());
        });
    }
});

