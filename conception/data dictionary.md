# **Data Dictionary — Messaging & Coaching Platform (SQL Server)**

This Data Dictionary describes all internal SQL Server entities used by the platform.
Messaging data (messages, topics, chat metadata) is **not stored** here, as it is fully handled by Tinode.

Each table includes:
- Definition  
- Field description  
- Data types  
- Constraints  
- Business rules  

---

# **1. Table: Users**

## **Description**
Stores the primary identity of all users (athletes, coaches, administrators).

## **Fields**

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| **Id** | INT | PK, Identity | Unique identifier for each user |
| **Email** | NVARCHAR(255) | Unique, Not Null | User email used for login |
| **PasswordHash** | NVARCHAR(512) | Not Null | Hashed password (PBKDF2, bcrypt, etc.) |
| **Role** | VARCHAR(20) | Not Null | "User", "Coach", or "Admin" |
| **CreatedAt** | DATETIME2 | Not Null | Account creation timestamp |
| **UpdatedAt** | DATETIME2 | Null | Last profile update |

## **Business Rules**
- Email must be unique.
- Password is never stored in plain text.
- Role determines access rights (RBAC).

---

# **2. Table: Profiles**

## **Description**
Stores personal details linked to a user.

## **Fields**

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| **Id** | INT | PK, Identity | Unique profile ID |
| **UserId** | INT | FK → Users(Id) | Linked user |
| **FirstName** | NVARCHAR(120) | Not Null | User first name |
| **LastName** | NVARCHAR(120) | Not Null | User last name |
| **BirthDate** | DATE | Null | Optional birth date |
| **Gender** | VARCHAR(20) | Null | Optional gender field |

## **Business Rules**
- One profile per user.
- Deleting a user deletes their profile (cascade).

---

# **3. Table: AthleteDetails**

## **Description**
Additional fields specific to athletes.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | INT | PK, Identity | Unique record |
| **UserId** | INT | FK → Users(Id) | Linked athlete |
| **WeightCategory** | VARCHAR(50) | Null | Athlete weight class |
| **NextCompetitionDate** | DATE | Null | Estimated next competition |

## **Business Rules**
- Only users with role = "User" (athlete) may have athlete details.
- Record is optional and created only for athletes.

---

# **4. Table: CoachDetails**

## **Description**
Stores coach-specific data.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | INT | PK, Identity | Unique record |
| **UserId** | INT | FK → Users(Id) | Linked coach |
| **MonthlyPrice** | DECIMAL(10,2) | Not Null | Monthly cost of coaching |
| **ActiveStripeSubscription** | BIT | Not Null | Whether the coach's Stripe subscription is active |

## **Business Rules**
- Only users with role = "Coach" may have this record.
- Subscription status must sync with Stripe.

---

# **5. Table: TinodeLink**

## **Description**
Stores the mapping between SQL users and Tinode users.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | INT | PK, Identity | Unique record |
| **UserId** | INT | FK → Users(Id) | Internal user |
| **TinodeUserId** | NVARCHAR(255) | Not Null, Unique | External Tinode account identifier |

## **Business Rules**
- Exactly one Tinode account per SQL user.
- TinodeUserId must be created during registration.

---

# **6. Table: Sessions**

## **Description**
Manages user login sessions across devices.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | INT | PK, Identity | Session ID |
| **UserId** | INT | FK → Users(Id) | Linked user |
| **AccessToken** | NVARCHAR(600) | Not Null | JWT access token |
| **RefreshToken** | NVARCHAR(600) | Not Null | Long-lived refresh token |
| **DeviceInfo** | NVARCHAR(255) | Null | Device name or model |
| **CreatedAt** | DATETIME2 | Not Null | Session creation |
| **ExpiresAt** | DATETIME2 | Not Null | Token expiration |

## **Business Rules**
- A user may have multiple sessions.
- Revoking a session invalidates its refresh token.

---

# **7. Table: Devices**

## **Description**
Stores registered user devices for push notifications and session management.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | INT | PK, Identity | Unique device record |
| **UserId** | INT | FK → Users(Id) | Linked user |
| **DeviceType** | VARCHAR(50) | Not Null | iOS, Android, Web |
| **DeviceToken** | NVARCHAR(400) | Null | Push notification token |
| **CreatedAt** | DATETIME2 | Not Null | Registration date |

## **Business Rules**
- DeviceToken is optional.
- Users may register multiple devices.

---

# **8. Table: Subscriptions**

## **Description**
Stores athlete-to-coach subscription relationships.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | INT | PK |
| **AthleteId** | INT | FK → Users(Id) | Athlete user |
| **CoachId** | INT | FK → Users(Id) | Coach user |
| **IsActive** | BIT | Not Null | Subscription status |
| **PaymentProvider** | VARCHAR(20) | Not Null | "Stripe" or "PayPal" |
| **CreatedAt** | DATETIME2 | Not Null | Start date |
| **ExpiresAt** | DATETIME2 | Null | End date |

## **Business Rules**
- An athlete can subscribe to only one coach at a time.
- Expiration or cancellation immediately disables premium access.

---

# **9. Table: AuditLogs**

## **Description**
Stores security and administrative events.

## **Fields**

| Field | Type | Constraints | Description |
|-------|-------|-------------|-------------|
| **Id** | BIGINT | PK, Identity | Log entry ID |
| **UserId** | INT | FK → Users(Id) | Optional linked user |
| **Action** | NVARCHAR(255) | Not Null | Action performed (LOGIN_FAILED, ROLE_CHANGE, etc.) |
| **Timestamp** | DATETIME2 | Not Null | Event time |
| **IpAddress** | VARCHAR(45) | Null | Request IP |
| **Metadata** | NVARCHAR(MAX) | Null | Optional contextual info |

## **Business Rules**
- Logs must never contain message content.
- Logs are stored for 6 to 12 months depending on compliance rules.

---

# **End of Document**
