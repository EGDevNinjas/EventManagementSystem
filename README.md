# 🎉 Event System

**Event System** is a full-featured platform for creating, managing, and attending events.  
It supports multiple user roles — Organizers, Attendees, Staff, and Admins — and includes real-time features, secure payments, and communication tools.

## 🚀 Features

### 👤 Roles & Permissions
- **Attendee:** Book tickets, check in with QR codes, receive updates, rate events
- **Organizer:** Create events, assign staff, view bookings and analytics
- **Staff:** Handle event check-ins and assist attendees
- **Admin:** Manage users, approve organizers, and oversee the whole system

### 🎫 Ticketing & Check-in
- Create events (public/private) with ticket limits and pricing
- Real-time booking and cancellation
- Unique QR code generation per ticket
- QR code scanning for check-in

### 💳 Payment Integration
- Secure online payments using **Stripe** or **PayPal**
- Transaction tracking and history
- Refund handling (if applicable)

### 🔔 Notifications
- **Email notifications** (e.g., booking confirmation, reminders)
- **SMS notifications** (e.g., event updates, last-minute alerts)
- Configurable templates per event

### 💬 Real-time Interaction
- Live announcements using **SignalR**
- Real-time chat between staff and attendees (optional)
- Instant updates for check-in status

### ⭐ Feedback & Rating System
- Attendees can rate events after attending
- Ratings visible to organizers and admins
- Analytics dashboard with feedback insights

### 📊 Admin & Organizer Dashboard
- Visual charts and statistics
- Role-based access and management tools
- Audit logs and user activity tracking

## 🧰 Tech Stack

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SignalR** for real-time communication
- **JWT Authentication**
- **SQL Server**
- **Stripe / PayPal** for payments
- **Email/SMS Gateways** (e.g., SendGrid, Twilio)
- **QR Code Generator**

