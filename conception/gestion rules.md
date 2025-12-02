# **Business Rules — Messaging & Coaching Platform (Tinode Externalized Chat)**  
*Enterprise-grade, complete and fully aligned with SQL Server + .NET + Tinode*

---

# **1. User Management Rules**

### **BR-U1 — Unique User Identity**
- Each user must have a unique system identifier.
- Email must be unique, verified, and cannot be changed after verification.

### **BR-U2 — Authentication Requirements**
- Authentication uses email + password and optional 2FA.
- Tokens must follow JWT standards:
  - Access Token: 15 minutes  
  - Refresh Token: 7–30 days  

### **BR-U3 — User Roles**
Roles are:
- **Admin** (`role_admin = 1`)
- **Athlete** (exists in `user_athlete`)
- **Coach** (exists in `user_coach`)

A user must be **either a Coach OR an Athlete**.

### **BR-U4 — Account States**
States:
- Active  
- Suspended  
- Deactivated  

Suspended/deactivated users lose all access.

---

# **2. Profile & Personal Data Rules**

### **BR-P1 — Mandatory Profile Data**
Each user must have:
- First name  
- Last name  
- Email  
- Role (coach/athlete)

### **BR-P2 — Personal Data Isolation**
- SQL Server stores all business data.
- Tinode stores messaging-only data.
- Sensitive data must not be duplicated on Tinode.

### **BR-P3 — GDPR Compliance**
Users must be able to:
- Request data export  
- Request account deletion  
- Withdraw consent  

---

# **3. Coaching & Athlete Rules**

### **BR-C1 — Exclusive Coaching Relationship**
- An athlete may have only **one active coach**.
- A coach may have multiple athletes.

### **BR-C2 — Coaching Link Lifecycle**
Link states:
- Active  
- Paused  
- Canceled  
- Completed  

### **BR-C3 — Visibility Rules**
- Athlete sees only their assigned coach.
- Coach sees only their assigned athletes.

### **BR-C4 — Coach Profile Requirements**
Coach profiles must include:
- biography  
- specialities  
- monthly price  
- rating  

---

# **4. Application Subscription Rules (Stripe)**

### **BR-SA1 — Mandatory App Subscription**
Every user must maintain an active subscription in `subscription_app_stripe`.

### **BR-SA2 — Expiration Effects**
Expiration causes:
- Loss of messaging access  
- Coach/athlete link deactivation  
- Tinode suspension  

### **BR-SA3 — Billing Synchronization**
- Stripe webhook events must update SQL subscription status.
- Failed payments trigger automatic suspension.

---

# **5. Coach Billing Rules (Stripe)**

### **BR-SC1 — Coaches Must Pay to Operate**
Coaches must maintain an active `subscription_coach_stripe`.

### **BR-SC2 — Suspension Effects**
If a coach loses subscription:
- Their profile is hidden  
- No new athlete links allowed  
- Existing links may be paused  

---

# **6. Athlete → Coach Monthly Payment Rules**

### **BR-AC1 — Required Athlete Subscription**
Athletes must have a valid recurring Stripe subscription to their coach.

### **BR-AC2 — Expiration Impact**
If the subscription expires:
- Coaching link is set to **Paused**  
- Athlete loses access to premium content  

### **BR-AC3 — One Active Subscription**
An athlete may only have one active coach subscription.

---

# **7. Moderation Rules**

### **BR-MOD1 — Blocking Behavior**
Blocking must:
- Disable Tinode messaging instantly  
- Hide existing topics  
- Prevent new notifications  

### **BR-MOD2 — Reporting Behavior**
Reports include:
- Reporter user  
- Reported user  
- Reason  
- Status  

States:
- Pending  
- Under_Review  
- Action_Taken  
- Rejected  

### **BR-MOD3 — Automatic Sanctions**
- 3 reports in 24h → Auto suspension  
- 5 reports in 7 days → Permanent ban review  

---

# **8. Messaging Rules (Tinode)**

### **BR-M1 — Conversation Creation**
A Tinode topic can be created only if:
- Both users exist in SQL  
- Both have active subscriptions  
- Neither is blocked  

### **BR-M2 — Message Delivery**
Tinode must handle:
- Real-time delivery  
- Read receipts  
- Typing indicators  

### **BR-M3 — Message Constraints**
Messages must follow:
- Max length  
- Allowed MIME  
- Max attachment size  
- No HTML/JS  

### **BR-M4 — Message Status**
Allowed statuses:
- Sent  
- Delivered  
- Read  
- Failed  
- Pending  

Only metadata may be stored in SQL.

---

# **9. Session & Token Rules**

### **BR-SES1 — Multi-Device Support**
Users may have multiple sessions.

### **BR-SES2 — Token Expiration**
- Access Token: 15 minutes  
- Refresh Token: 7–30 days  

### **BR-SES3 — Logout Behavior**
Logout must:
- Delete refresh token  
- Invalidate push token  
- Close Tinode session  

---

# **10. Tinode Integration Rules**

### **BR-TIN1 — Identity Consistency**
Each SQL user must have a single Tinode entry in `tinode_account`.

### **BR-TIN2 — No Orphan Accounts**
No Tinode record may exist without a SQL user.

### **BR-TIN3 — Suspension Sync**
Suspension in SQL → suspended in Tinode.

---

# **11. Security Rules**

### **BR-S1 — Encryption & Transport**
Use:
- HTTPS  
- Secure WebSockets (WSS)

### **BR-S2 — Password Security**
Use secure hashing such as:
- PBKDF2  
- BCrypt  
- Argon2  

### **BR-S3 — Log Restrictions**
Logs must not include:
- Message bodies  
- Plain tokens  
- Passwords  
- Sensitive data  

### **BR-S4 — Audit Requirements**
Audit logs track:
- Logins  
- Failed logins  
- Suspensions  
- Role changes  
- Subscription changes  

---

# **12. System Reliability & Recovery**

### **BR-R1 — Tinode Retry Policy**
Failed message delivery must trigger retries.

### **BR-R2 — Backup Strategy**
- SQL backups every 24h  
- Tinode snapshots every 24h  
- 30-day retention  

### **BR-R3 — High Availability**
Recommended (optional):
- Load balancing  
- SQL replication  
- Tinode clustering  

---

# **13. Logging & Monitoring**

### **BR-L1 — Monitoring Requirements**
Monitor:
- Authentication failures  
- Tinode errors  
- Backend exceptions  
- Slow queries  
- Message latency  

### **BR-L2 — Centralized Logging**
Use:
- ELK Stack  
- or Azure Application Insights  

---

# **End of Document**
