document.addEventListener("DOMContentLoaded", function () {
    // Initialize WOW.js for scroll animations
    const wow = new WOW({
        boxClass: "wow", // class name that triggers the animation
        animateClass: "animate__animated", // class name for animated elements
        offset: 100, // offset in pixels from the viewport
        mobile: true, // enable animations on mobile devices
        live: true, // enable/disable MutationObserver
    });
    wow.init();

    // Custom scroll behavior to re-trigger animations
    let ticking = false;
    const animatedElements = new Set();

    function updateAnimations() {
        const elements = document.querySelectorAll(".wow");
        const windowHeight = window.innerHeight;
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        elements.forEach((element) => {
            const rect = element.getBoundingClientRect();
            const elementTop = rect.top + scrollTop;
            const elementBottom = elementTop + element.offsetHeight;
            const viewportTop = scrollTop;
            const viewportBottom = scrollTop + windowHeight;

            // Check if element is in viewport
            const isInViewport = rect.top < windowHeight - 100 && rect.bottom > 100;

            if (isInViewport) {
                // Element is in viewport - ensure animation is active
                if (!element.classList.contains("animate__animated")) {
                    element.classList.add("animate__animated");
                    element.style.visibility = "visible";
                    animatedElements.add(element);
                }
            } else {
                // Element is out of viewport - reset for re-animation
                if (animatedElements.has(element)) {
                    element.classList.remove("animate__animated");
                    element.style.visibility = "hidden";
                    animatedElements.delete(element);
                }
            }
        });

        ticking = false;
    }

    function requestTick() {
        if (!ticking) {
            requestAnimationFrame(updateAnimations);
            ticking = true;
        }
    }

    window.addEventListener("scroll", requestTick);

    // Initial check
    setTimeout(updateAnimations, 100);

    // Calendar functionality
    const dateInput = document.getElementById("date");
    const calendarContainer = document.getElementById("calendar-container");
    const calendarIcon = document.getElementById("calendar-icon");
    const calendarDays = document.getElementById("calendar-days");
    const calendarMonthYear = document.getElementById("calendar-month-year");
    const prevMonthBtn = document.getElementById("prev-month");
    const nextMonthBtn = document.getElementById("next-month");

    // Time slots functionality
    const timeSlots = document.querySelectorAll(".time-slot");
    const selectedTimeInput = document.getElementById("selected-time");

    // Modal elements
    const modal = document.getElementById("success-modal");
    const closeModalBtn = document.getElementById("close-modal");
    const form = document.getElementById("appointment-form");

    let currentDate = new Date();
    let selectedDate = null;

    // Calendar months in Vietnamese
    const months = [
        "Tháng 1",
        "Tháng 2",
        "Tháng 3",
        "Tháng 4",
        "Tháng 5",
        "Tháng 6",
        "Tháng 7",
        "Tháng 8",
        "Tháng 9",
        "Tháng 10",
        "Tháng 11",
        "Tháng 12",
    ];

    // Service names mapping
    const serviceNames = {
        exam: "Khám tổng quát",
        cleaning: "Vệ sinh răng miệng",
        whitening: "Tẩy trắng răng",
        braces: "Niềng răng",
        crown: "Bọc răng sứ",
        implant: "Cấy ghép implant",
    };

    // Initialize calendar
    function initCalendar() {
        updateCalendar();

        // Event listeners for calendar
        dateInput.addEventListener("click", (e) => {
            e.stopPropagation();
            toggleCalendar();
        });

        calendarIcon.addEventListener("click", (e) => {
            e.stopPropagation();
            toggleCalendar();
        });

        prevMonthBtn.addEventListener("click", () => {
            currentDate.setMonth(currentDate.getMonth() - 1);
            updateCalendar();
        });

        nextMonthBtn.addEventListener("click", () => {
            currentDate.setMonth(currentDate.getMonth() + 1);
            updateCalendar();
        });

        // Prevent calendar from closing when clicking inside it
        calendarContainer.addEventListener("click", (e) => {
            e.stopPropagation();
        });

        // Close calendar when clicking outside
        document.addEventListener("click", (e) => {
            if (
                calendarContainer.classList.contains("active") &&
                !calendarContainer.contains(e.target) &&
                !dateInput.contains(e.target) &&
                !calendarIcon.contains(e.target)
            ) {
                calendarContainer.classList.remove("active");
            }
        });
    }

    function toggleCalendar(e) {
        calendarContainer.classList.toggle("active");
    }

    function updateCalendar() {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();

        calendarMonthYear.textContent = `${months[month]} ${year}`;

        // Clear previous days
        calendarDays.innerHTML = "";

        // Get first day of month and number of days
        const firstDay = new Date(year, month, 1);
        const lastDay = new Date(year, month + 1, 0);
        const daysInMonth = lastDay.getDate();
        const startingDayOfWeek = firstDay.getDay();

        // Get today's date (reset time to compare dates only)
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        // Add empty cells for days before the first day of the month
        for (let i = 0; i < startingDayOfWeek; i++) {
            const emptyDay = document.createElement("div");
            emptyDay.className = "calendar-day other-month";
            calendarDays.appendChild(emptyDay);
        }

        // Add days of the month
        for (let day = 1; day <= daysInMonth; day++) {
            const dayElement = document.createElement("div");
            dayElement.className = "calendar-day";
            dayElement.textContent = day;

            const currentDayDate = new Date(year, month, day);
            currentDayDate.setHours(0, 0, 0, 0);

            // Check if it's today
            if (currentDayDate.getTime() === today.getTime()) {
                dayElement.classList.add("today");
            }

            // Disable past dates
            if (currentDayDate < today) {
                dayElement.classList.add("disabled");
            } else {
                dayElement.addEventListener("click", () =>
                    selectDate(currentDayDate, dayElement)
                );
            }

            // Check if this date is selected
            if (selectedDate && currentDayDate.getTime() === selectedDate.getTime()) {
                dayElement.classList.add("selected");
            }

            calendarDays.appendChild(dayElement);
        }
    }

    function selectDate(date, element) {
        // Remove previous selection
        document.querySelectorAll(".calendar-day.selected").forEach((day) => {
            day.classList.remove("selected");
        });

        // Add selection to clicked element
        element.classList.add("selected");

        // Create a new date object to avoid mutation
        selectedDate = new Date(date.getTime());

        // Format date for input
        const formattedDate = selectedDate.toLocaleDateString("vi-VN", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
        });

        dateInput.value = formattedDate;
        calendarContainer.classList.remove("active");

        // Update time slots availability based on selected date
        updateTimeSlots(selectedDate);
    }

    function updateTimeSlots(date) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const selectedDateOnly = new Date(date.getTime());
        selectedDateOnly.setHours(0, 0, 0, 0);

        const isToday = selectedDateOnly.getTime() === today.getTime();
        const currentHour = new Date().getHours();

        // Get existing appointments for the selected date
        const existingAppointments = JSON.parse(
            localStorage.getItem("appointments") || "[]"
        );
        const dateKey = selectedDateOnly.toDateString();

        timeSlots.forEach((slot) => {
            const slotTime = parseInt(slot.dataset.time.split(":")[0]);
            const appointmentKey = dateKey + "_" + slot.dataset.time;

            // Reset classes
            slot.classList.remove("disabled", "booked");
            slot.style.pointerEvents = "auto";
            slot.title = "Có thể đặt lịch";

            // Disable past time slots for today
            if (isToday && slotTime <= currentHour) {
                slot.classList.add("disabled");
                slot.style.pointerEvents = "none";
                slot.title = "Giờ này đã qua";
            }
            // Mark booked slots
            else if (existingAppointments.includes(appointmentKey)) {
                slot.classList.add("booked");
                slot.style.pointerEvents = "none";
                slot.title = "Đã có người đặt";
            }
        });
    }

    // Time slot selection
    timeSlots.forEach((slot) => {
        slot.addEventListener("click", function () {
            if (this.classList.contains("disabled")) return;

            // Remove previous selection
            timeSlots.forEach((s) => s.classList.remove("selected"));

            // Add selection to clicked slot
            this.classList.add("selected");
            selectedTimeInput.value = this.dataset.time;
        });
    });

    // Phone number formatting and validation
    const phoneInput = document.getElementById("phone");
    phoneInput.addEventListener("input", function (e) {
        let value = e.target.value.replace(/\D/g, "");
        if (value.length > 10) {
            value = value.slice(0, 10);
        }

        // Format phone number as user types
        if (value.length >= 6) {
            e.target.value =
                value.slice(0, 3) + " " + value.slice(3, 6) + " " + value.slice(6);
        } else if (value.length >= 3) {
            e.target.value = value.slice(0, 3) + " " + value.slice(3);
        } else {
            e.target.value = value;
        }
    });

    // Name validation
    const nameInput = document.getElementById("name");
    nameInput.addEventListener("input", function (e) {
        // Remove numbers and special characters except spaces, dots, and Vietnamese characters
        e.target.value = e.target.value.replace(/[^a-zA-ZÀ-ỹ\s.]/g, "");
    });

    // Form submission
    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const formData = new FormData(form);
        const name = formData.get("name").trim();
        const phone = formData.get("phone").replace(/\s/g, ""); // Remove spaces for validation
        const email = formData.get("email").trim();
        const time = formData.get("time");
        const service = formData.get("service");

        // Enhanced validation with specific error messages
        const errors = [];

        if (!name) {
            errors.push("Vui lòng nhập họ và tên");
        } else if (name.length < 2) {
            errors.push("Họ tên phải có ít nhất 2 ký tự");
        }

        if (!phone) {
            errors.push("Vui lòng nhập số điện thoại");
        } else if (phone.length < 10) {
            errors.push("Số điện thoại phải có ít nhất 10 số");
        } else if (!/^(0[3|5|7|8|9])+([0-9]{8})$/.test(phone)) {
            errors.push(
                "Số điện thoại không hợp lệ (phải bắt đầu bằng 03, 05, 07, 08, 09)"
            );
        }
        if (!/^(0[3|5|7|8|9])+([0-9]{8})$/.test(phone)) {
            errors.push(
                "Số điện thoại không hợp lệ (phải bắt đầu bằng 03, 05, 07, 08, 09)"
            );
        }

        if (!selectedDate) {
            errors.push("Vui lòng chọn ngày khám");
        } else {
            // Check if selected date is not in the past
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            const selectedDateOnly = new Date(selectedDate.getTime());
            selectedDateOnly.setHours(0, 0, 0, 0);

            if (selectedDateOnly < today) {
                errors.push("Không thể đặt lịch cho ngày đã qua");
            }

            // Check if selected date is too far in the future (max 3 months)
            const maxDate = new Date();
            maxDate.setMonth(maxDate.getMonth() + 3);
            maxDate.setHours(0, 0, 0, 0);

            if (selectedDateOnly > maxDate) {
                errors.push("Chỉ có thể đặt lịch tối đa 3 tháng trước");
            }
        }

        if (!time) {
            errors.push("Vui lòng chọn giờ khám");
        }

        if (!service) {
            errors.push("Vui lòng chọn dịch vụ");
        }

        // Email validation (optional but if provided must be valid)
        if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            errors.push("Email không hợp lệ");
        }

        // Check for appointment conflicts
        const appointmentKey = selectedDate
            ? selectedDate.toDateString() + "_" + time
            : null;
        const existingAppointments = JSON.parse(
            localStorage.getItem("appointments") || "[]"
        );

        if (appointmentKey && existingAppointments.includes(appointmentKey)) {
            errors.push(
                "Thời gian này đã có người đặt, vui lòng chọn thời gian khác"
            );
        }

        if (errors.length > 0) {
            showErrorModal(errors);
            return;
        }

        // Save appointment to localStorage
        if (appointmentKey) {
            existingAppointments.push(appointmentKey);
            localStorage.setItem(
                "appointments",
                JSON.stringify(existingAppointments)
            );
        }

        // Show loading state
        const submitBtn = form.querySelector(".btn-submit");
        const originalText = submitBtn.innerHTML;
        submitBtn.innerHTML =
            '<i class="fas fa-spinner fa-spin"></i> <span>Đang xử lý...</span>';
        submitBtn.disabled = true;

        // Simulate API call
        setTimeout(() => {
            // Show success modal
            document.getElementById("confirm-name").textContent = name;
            document.getElementById("confirm-phone").textContent = phoneInput.value; // Keep formatted version

            // Show email if provided
            const emailRow = document.getElementById("confirm-email-row");
            if (email) {
                document.getElementById("confirm-email").textContent = email;
                emailRow.style.display = "block";
            } else {
                emailRow.style.display = "none";
            }

            document.getElementById("confirm-date").textContent =
                selectedDate.toLocaleDateString("vi-VN", {
                    weekday: "long",
                    year: "numeric",
                    month: "long",
                    day: "numeric",
                });
            document.getElementById("confirm-time").textContent = time;
            document.getElementById("confirm-service").textContent =
                serviceNames[service];

            modal.style.display = "flex";

            // Reset form
            form.reset();
            selectedDate = null;
            selectedTimeInput.value = "";

            // Reset UI states
            document.querySelectorAll(".calendar-day.selected").forEach((day) => {
                day.classList.remove("selected");
            });
            timeSlots.forEach((slot) => slot.classList.remove("selected"));

            // Reset button
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;

            // Update appointment counter
            updateAppointmentCounter();
        }, 1500); // Simulate network delay
    });

    // Error modal function
    function showErrorModal(errors) {
        // Create error modal if it doesn't exist
        let errorModal = document.getElementById("error-modal");
        if (!errorModal) {
            errorModal = document.createElement("div");
            errorModal.id = "error-modal";
            errorModal.className = "modal";
            errorModal.innerHTML = `
                <div class="modal-content">
                    <div class="modal-header" style="background: linear-gradient(135deg, #e74c3c, #c0392b);">
                        <div class="error-icon">
                            <i class="fas fa-exclamation-triangle"></i>
                        </div>
                        <h3>Vui lòng kiểm tra lại thông tin! ⚠️</h3>
                    </div>
                    <div class="modal-body">
                        <div id="error-list"></div>
                    </div>
                    <button id="close-error-modal" class="btn-primary" style="background: linear-gradient(135deg, #e74c3c, #c0392b);">Đóng</button>
                </div>
            `;
            document.body.appendChild(errorModal);

            // Add close event listener
            errorModal
                .querySelector("#close-error-modal")
                .addEventListener("click", () => {
                    errorModal.style.display = "none";
                });

            errorModal.addEventListener("click", (e) => {
                if (e.target === errorModal) {
                    errorModal.style.display = "none";
                }
            });
        }

        // Update error list
        const errorList = errorModal.querySelector("#error-list");
        errorList.innerHTML = errors
            .map(
                (error) =>
                    `<p style="color: #e74c3c; margin-bottom: 10px;"><i class="fas fa-times-circle"></i> ${error}</p>`
            )
            .join("");

        errorModal.style.display = "flex";
    }

    // Close modal
    closeModalBtn.addEventListener("click", function () {
        modal.style.display = "none";
    });

    // Close modal when clicking outside
    modal.addEventListener("click", function (e) {
        if (e.target === modal) {
            modal.style.display = "none";
        }
    });

    // Smooth scrolling for navigation links
    document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
        anchor.addEventListener("click", function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute("href"));
            if (target) {
                target.scrollIntoView({
                    behavior: "smooth",
                    block: "start",
                });
            }
        });
    });

    // Header scroll effect
    let lastScrollTop = 0;
    const header = document.querySelector("header");

    window.addEventListener("scroll", function () {
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        if (scrollTop > lastScrollTop && scrollTop > 100) {
            header.style.transform = "translateY(-100%)";
        } else {
            header.style.transform = "translateY(0)";
        }

        lastScrollTop = scrollTop;
    });

    // Floating particles effect
    function createParticle() {
        const particle = document.createElement("div");
        particle.style.cssText = `
            position: fixed;
            width: 6px;
            height: 6px;
            background: linear-gradient(45deg, #4ECDC4, #45B7AF);
            border-radius: 50%;
            pointer-events: none;
            z-index: 1;
            opacity: 0.7;
            box-shadow: 0 0 10px rgba(78, 205, 196, 0.5);
        `;

        particle.style.left = Math.random() * 100 + "%";
        particle.style.top = "100vh";

        document.body.appendChild(particle);

        const animation = particle.animate(
            [
                {
                    transform: "translateY(0) rotate(0deg) scale(1)",
                    opacity: 0.7,
                },
                {
                    transform: `translateY(-100vh) rotate(${Math.random() * 360
                        }deg) scale(${Math.random() * 2 + 1})`,
                    opacity: 0,
                },
            ],
            {
                duration: Math.random() * 4000 + 3000,
                easing: "cubic-bezier(0.25, 0.46, 0.45, 0.94)",
            }
        );

        animation.onfinish = () => {
            particle.remove();
        };
    }

    // Create particles periodically
    setInterval(createParticle, 400);

    // Initialize calendar
    initCalendar();

    // Update appointment counter on page load
    updateAppointmentCounter();

    function updateAppointmentCounter() {
        const existingAppointments = JSON.parse(
            localStorage.getItem("appointments") || "[]"
        );
        const counter = document.createElement("div");
        counter.id = "appointment-counter";
        counter.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: var(--gradient);
            color: white;
            padding: 10px 15px;
            border-radius: 25px;
            font-weight: 600;
            font-size: 14px;
            box-shadow: var(--shadow);
            z-index: 1000;
            display: ${existingAppointments.length > 0 ? "block" : "none"};
        `;
        counter.innerHTML = `<i class="fas fa-calendar-check"></i> ${existingAppointments.length} lịch đã đặt`;

        // Remove existing counter
        const existingCounter = document.getElementById("appointment-counter");
        if (existingCounter) {
            existingCounter.remove();
        }

        document.body.appendChild(counter);
    }

    // Add some cute hover effects
    document.querySelectorAll(".service-card, .about-card").forEach((card) => {
        card.addEventListener("mouseenter", function () {
            this.style.transform = "translateY(-15px) scale(1.03)";
        });

        card.addEventListener("mouseleave", function () {
            this.style.transform = "translateY(0) scale(1)";
        });
    });

    // Button hover effects
    document.querySelectorAll(".btn-primary, .btn-submit").forEach((btn) => {
        btn.addEventListener("mouseenter", function () {
            this.style.transform = "translateY(-4px) scale(1.05)";
        });

        btn.addEventListener("mouseleave", function () {
            this.style.transform = "translateY(0) scale(1)";
        });
    });
});
