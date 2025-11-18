Business Rules – Messaging & Coaching Application (English Version)

This document contains the complete and updated business rules of the application, rewritten for clarity and adapted to the architecture using Tinode for messaging.

## 1. Visitor Rules

1.1 Access

A visitor can access the homepage of the application.

A visitor can view the list of services offered.

A visitor cannot access premium or messaging features.

1.2 Registration

A visitor can create an account.

During registration, the visitor must choose a role:

Coach

Athlete

The visitor must fill out a form with required personal information.

After registration:

A corresponding Tinode account must also be created.

The Tinode User ID must be linked to the application's user ID.

## 2. Authentication Rules

2.1 Login

A user can log in using email + password.

Authentication is performed using JWT (access + refresh tokens).

Access token expires quickly (e.g., 15 minutes).

Refresh token expires later (e.g., 7 days).

2.2 Forgot Password

A user can request a password reset link via email.

The link must expire after a short period.

2.3 Account Lockout

After 8 failed login attempts, the account is temporarily locked.

Lock duration: configurable (e.g., 15 minutes).

2.4 Multi‑Factor Authentication (optional)

A user can enable 2FA using:

Email code

SMS code

Authenticator app

## 3. User Profile Rules

3.1 Editable Information

A user can update:

First name

Last name

Email

Password

3.2 Account Management

A user can deactivate their account.

A deactivated account can be reactivated within 30 days.

A user may request permanent account deletion.

## 4. Athlete‑Specific Rules

4.1 Required Information

An athlete must provide:

Weight category

Next competition date

4.2 Modifications

An athlete can update weight category.

An athlete can update competition date.

4.3 Subscription Rules

An athlete must have an active PayPal subscription to access premium features.

A subscription can be cancelled at any time.

When the subscription ends or expires:

Premium features are disabled.

The athlete cannot send messages to coaches.

## 5. Coach‑Specific Rules

5.1 Required Information

A coach must define their monthly subscription price.

5.2 Subscription Status

A coach must maintain an active Stripe subscription.

If the coach's Stripe subscription becomes inactive:

The coach loses access to coaching tools.

Athletes cannot subscribe to them.

## 6. Admin Rules

6.1 Account Moderation

The admin can delete any user account.

The admin can ban a user.

6.2 Reporting System

Admin receives reports sent by users.

Admin can take actions such as:

Warning the user

Temporary or permanent ban

## 7. Subscription & Payment Rules

7.1 Athlete Payments (PayPal)

Payments for coaches made by athletes must be processed via PayPal.

Each PayPal subscription must store:

Start date

End date

Status

Price

7.2 Coach Payments (Stripe)

Coaches must pay Stripe to maintain premium coaching status.

The application must store the Stripe subscription ID.

When Stripe notifies via webhook:

Update subscription status in the database.

7.3 Cancellation Rules

Users can cancel subscriptions at any time.

Refunds follow the policy of the payment provider.

## 8. Messaging Rules (Tinode‑Based)

8.1 Access

Only athletes with an active PayPal subscription can chat with their assigned coach.

Only coaches with an active Stripe subscription can respond to athletes.

8.2 Message Sending

Messages are NOT stored in the application database.

All messages are stored and delivered by Tinode.

The application must store only:

Tinode user ID (for each user)

Tinode topic IDs (if needed)

8.3 Operations

Users can:

Send text messages

Delete their own messages

See read receipts

Report inappropriate messages

8.4 Real‑Time Features

Real‑time delivery

Typing indicators

Online/offline status

Read receipts

Message history

All real‑time features are managed by Tinode, not by your backend.
