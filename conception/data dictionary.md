# Data Dictionary

This document describes all SQL Server tables in your final validated model.
It covers:

- Table description
- Field definitions
- Data types
- Constraints
- Business rules
- Links to Tinode (external)

---

# 1. **Table: User\_**

Stores the main identity of every account (coach or athlete).

| Field                      | Type         | Constraints                  | Description             |
| -------------------------- | ------------ | ---------------------------- | ----------------------- |
| Id_user                    | INT          | PK, Identity                 | Unique user identifier  |
| first_name                 | VARCHAR(20)  | Not Null                     | First name              |
| last_name                  | VARCHAR(20)  | Not Null                     | Last name               |
| email                      | VARCHAR(50)  | Unique, Not Null             | Login email             |
| password                   | VARCHAR(255) | Not Null                     | Hashed password         |
| role_admin                 | BIT          | Not Null                     | True = admin role       |
| id_subscription_app_stripe | INT          | FK → subscription_app_stripe | User's app subscription |

**Business Rules**

- Every user must have an active or inactive app subscription row.
- Exactly one user type extension (coach or athlete) exists.

---

# 2. **Table: user_athlete**

Holds specific data for athlete accounts.

| Field                        | Type        | Constraints  | Description             |
| ---------------------------- | ----------- | ------------ | ----------------------- |
| id_user_athlete              | INT         | PK, Identity | Athlete profile ID      |
| weight_category              | VARCHAR(20) | Not Null     | Weight category         |
| id_coach_subscription_stripe | INT         | FK           | Subscription to a coach |
| Id_user                      | INT         | FK, Unique   | Linked base user        |

**Business Rules**

- Athlete must correspond to exactly one User\_.
- Cannot exist without a linked User\_.

---

# 3. **Table: user_coach**

Coach-specific data.

| Field                        | Type          | Constraints  | Description                  |
| ---------------------------- | ------------- | ------------ | ---------------------------- |
| id_user_coach                | INT           | PK, Identity | Coach profile ID             |
| month_price                  | DECIMAL(15,2) | Not Null     | Monthly coaching price       |
| biography                    | TEXT          | Not Null     | Coach description            |
| specialities                 | VARCHAR(200)  | Not Null     | Coaching specialities        |
| completed_sessions           | INT           | Not Null     | Number of completed sessions |
| rating                       | DECIMAL(3,2)  | Not Null     | Rating value                 |
| id_coach_subscription_stripe | INT           | FK           | Coach paying subscription    |
| Id_user                      | INT           | FK, Unique   | Linked base user             |

**Business Rules**

- Only 1 coach record per user.
- Cannot exist without a linked User\_.

---

# 4. **Table: subscription_app_stripe**

Stores subscription of the user to the application itself.

| Field                      | Type          | Constraints  | Description            |
| -------------------------- | ------------- | ------------ | ---------------------- |
| id_subscription_app_stripe | INT           | PK, Identity | Unique ID              |
| id_stripe                  | VARCHAR(50)   | Not Null     | Stripe subscription ID |
| start_subscription         | DATE          | Not Null     | Subscription start     |
| end_subscription           | DATE          | Not Null     | Subscription end       |
| status_subscription        | VARCHAR(20)   | Not Null     | active / canceled      |
| price                      | DECIMAL(15,2) | Not Null     | Price paid             |
| created_at                 | DATE          | Not Null     | Creation timestamp     |

---

# 5. **Table: subscription_coach_stripe**

Stores the subscription of an athlete to a coach.

| Field                        | Type          | Constraints  | Description            |
| ---------------------------- | ------------- | ------------ | ---------------------- |
| id_coach_subscription_stripe | INT           | PK, Identity | Unique ID              |
| id_stripe                    | VARCHAR(50)   | Not Null     | Stripe subscription ID |
| start_subscription           | DATE          | Not Null     | Start date             |
| end_subscription             | DATE          | Not Null     | End date               |
| status_subscription          | VARCHAR(20)   | Not Null     | Subscription status    |
| price                        | DECIMAL(15,2) | Not Null     | Monthly price paid     |
| created_at                   | DATE          | Not Null     | Creation timestamp     |

---

# 6. **Table: link**

Represents the relationship between a coach and an athlete.

| Field           | Type        | Constraints | Description             |
| --------------- | ----------- | ----------- | ----------------------- |
| id_user_athlete | INT         | PK, FK      | Linked athlete          |
| id_user_coach   | INT         | PK, FK      | Linked coach            |
| start_date      | DATE        | Not Null    | Link start              |
| end_date        | DATE        | Not Null    | Link end                |
| status          | VARCHAR(50) | Not Null    | active / paused / ended |

**Business Rules**

- An athlete can link to only one coach at a time.
- A coach can manage multiple athletes.

---

# 7. **Table: user_block**

Represents user-to-user blocking actions.

| Field           | Type        | Constraints  | Description               |
| --------------- | ----------- | ------------ | ------------------------- |
| id_block        | INT         | PK, Identity | Block entry               |
| id_user_blocker | INT         | FK           | User performing the block |
| id_user_blocked | INT         | FK           | User being blocked        |
| status          | BIT         | Null         | Optional flag             |
| reason          | VARCHAR(50) | Null         | Optional reason           |
| created_at      | DATETIME    | Not Null     | Block timestamp           |

**Business Rules**

- A user may block multiple users.
- Duplicate block entries may be prevented by the backend.

---

# 8. **Table: user_report**

Handles moderation reports.

| Field            | Type         | Constraints  | Description          |
| ---------------- | ------------ | ------------ | -------------------- |
| id_report        | INT          | PK, Identity | Moderation report    |
| reason           | VARCHAR(200) | Not Null     | Report reason        |
| created_at       | DATETIME     | Not Null     | Report creation      |
| status           | VARCHAR(20)  | Not Null     | pending / resolved   |
| id_user_reporter | INT          | FK           | User who reports     |
| id_user_reported | INT          | FK           | User who is reported |

**Business Rules**

- A user may report many users.
- Moderators update report status.

---

# 9. **Table: blocage**

Associative table linking users to block actions.

| Field    | Type | Constraints | Description   |
| -------- | ---- | ----------- | ------------- |
| Id_user  | INT  | PK, FK      | User affected |
| id_block | INT  | PK, FK      | Block record  |

---

# 10. **Table: report**

Associative table linking users to moderation reports.

| Field     | Type | Constraints | Description           |
| --------- | ---- | ----------- | --------------------- |
| Id_user   | INT  | PK, FK      | User tied to a report |
| id_report | INT  | PK, FK      | Moderation report     |

---

# 11. **Table: user_session**

Stores login sessions.

| Field       | Type        | Constraints  | Description          |
| ----------- | ----------- | ------------ | -------------------- |
| id_session  | INT         | PK, Identity | Session ID           |
| device_type | VARCHAR(50) | Not Null     | Device category      |
| ip_adress   | VARCHAR(50) | Not Null     | IP address           |
| created_at  | DATETIME    | Not Null     | Session creation     |
| expires_at  | DATETIME    | Not Null     | Expiration timestamp |
| Id_user     | INT         | FK           | Linked user          |

---

# 12. **Table: user_refresh_token**

Refresh token store.

| Field         | Type         | Constraints  | Description |
| ------------- | ------------ | ------------ | ----------- |
| id_refresh    | INT          | PK, Identity | Token ID    |
| refresh_token | VARCHAR(250) | Not Null     | Token value |
| expires_at    | DATETIME     | Not Null     | Expiry date |
| Id_user       | INT          | FK           | Linked user |

---

# 13. **Table: tinode_account**

Maps internal users to Tinode user accounts.

| Field             | Type         | Constraints  | Description        |
| ----------------- | ------------ | ------------ | ------------------ |
| id_tinode_account | INT          | PK, Identity | Mapping ID         |
| tinode_user_id    | VARCHAR(250) | Not Null     | Tinode external ID |
| created_at        | DATETIME     | Not Null     | Link creation date |
| Id_user           | INT          | FK           | Linked user        |

---

# 14. **Table: audit_log**

Stores administrator and system audit events.

| Field       | Type         | Constraints  | Description                   |
| ----------- | ------------ | ------------ | ----------------------------- |
| id_log      | INT          | PK, Identity | Event ID                      |
| action      | VARCHAR(100) | Not Null     | Action name                   |
| description | TEXT         | Not Null     | Details                       |
| created_at  | DATETIME     | Not Null     | Timestamp                     |
| Id_user     | INT          | FK           | User who performed the action |

---

# **End of Document**
