# **Security Strategy — Messaging Platform**

This document describes the security strategy of the application.

- Protection of user data  
- Secure communication  
- Role management (Admin / Coach / Athlete)  
- Integration with Tinode (externalized chat)  
- Authentication & authorization  
- Compliance (GDPR)  
- Moderation (report/block)  

---

# **1. Identity & Access Management**

## **1.1. User Identity**
- SQL Server is the **main identity system**.
- Each account has a unique ID and a verified email.
- Passwords are never stored in plain text (secure hash algorithm).

## **1.2. Login & Authentication**
- The backend uses **JWT tokens** for authentication.
- Access token = short validity (≈ 15–60 min)  
- Refresh token = longer validity (≈ 7–30 days)
- Users can log in from several devices (phone + web).

## **1.3. Role Management**
The application defines 3 roles:

- **Admin**
- **Coach**
- **Athlete**

Each role has specific permissions.  
Permissions are always checked in the backend before executing a request.

---

# **2. Authorization Rules**

## **2.1. Least Privilege**
Every user only accesses the resources they own.  
Examples:
- A coach cannot access another coach’s data  
- An athlete cannot modify someone else’s sessions  
- An admin cannot read chat messages

## **2.2. Resource Ownership**
Before accessing a resource, the backend verifies:
- User identity  
- Role compatibility  
- Ownership of the resource (example: athlete accessing their own subscription)

---

# **3. Secure Communication**

## **3.1. HTTPS Everywhere**
All communication between:
- Mobile/Web apps ↔ API  
- API ↔ SQL Server  
- API ↔ Tinode  

Must use **HTTPS / WSS**.

## **3.2. Transport Security**
- Cookies and tokens are transmitted over secure channels only.
- No sensitive data in URLs or logs.

---

# **4. Data Protection & Privacy**

## **4.1. SQL Server Data**
SQL Server stores only:
- User info  
- Subscriptions (App and Coaching)  
- Sessions  
- Reports & blocks  
- Audit logs  
**Not messages.**

Sensitive data (passwords, tokens) is always protected.

## **4.2. Tinode Message Management**
Tinode is responsible for:
- Message storage  
- Encryption  
- Chat topics  
- Delivery/read receipts  

**The backend and SQL never store message content.**

## **4.3. GDPR**
Users must be able to:
- Export their data  
- Delete their account (except legal logs)  
- Request information about how their data is used  

---

# **5. Session & Token Security**

## **5.1. Session Management**
Each device has its own session.  
The system allows:
- Manual logout  
- Revocation of a session  
- Automatic expiration

## **5.2. Refresh Token Protection**
A refresh token is:
- Unique  
- Stored securely  
- Invalidated when a session is closed  

---

# **6. Moderation & Abuse Prevention**

## **6.1. Blocking System**
When A blocks B:
- Messages from B are no longer delivered by Tinode  
- Conversation topics are muted  
- Blocking information is stored in SQL  

## **6.2. User Reports**
Each report contains:
- Reporter ID  
- Reported ID  
- Reason  
- Status (pending, reviewed, action taken)

Admins can:
- Suspend users  
- Deactivate accounts  
- Review report history

---

# **7. Billing & Subscription Security**

## **7.1. Application Subscription**
Users must have an active app subscription (Stripe) to:
- Use the platform  
- Access messaging  
- Access coach features  

## **7.2. Coaching Subscription**
Athletes subscribe monthly to their coach.  
If payment fails:
- Coaching link becomes inactive  
- Messages and features are temporarily blocked

## **7.3. Stripe Integration**
The backend receives webhook events for:
- Payment success  
- Payment failure  
- Subscription renewal  

Only Stripe IDs (not card data) are stored.

---

# **8. Logging & Monitoring**

## **8.1. Audit Logs**
The system logs:
- Login attempts  
- Account creation  
- Role changes  
- Suspensions  
- Tinode account creation  

Logs do **not** contain:
- Passwords  
- Tokens  
- Message content  

## **8.2. Error Monitoring**
The backend records:
- API errors  
- Tinode connection failures  
- Unexpected exceptions  

These logs help with debugging and incident response.

---

# **9. Backup & Recovery**

## **9.1. SQL Backups**
- Daily database backups  
- Secure storage  
- Retention: ~30 days  

## **9.2. Tinode Backup**
- Tinode handles its own message storage  
- Database snapshot every 24h

## **9.3. Recovery Procedure**
In case of incident:
- Restore latest SQL backup  
- Recreate Tinode users & sync metadata  
- Restart API services  

---

# **10. Incident Response (Simplified)**

1. Detect issue (logs, alerts, user reports)  
2. Identify severity (low/medium/high)  
3. Contain the issue (disable accounts, revoke tokens)  
4. Fix the cause (patch, config update)  
5. Document what happened  
6. Inform users if necessary  

---

# **End of Document**
