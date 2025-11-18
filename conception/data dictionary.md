# Data Dictionary

🏛️ Table : admin
Attribut Type Contraintes
Id_admin INT (AUTO) PK
email VARCHAR(50) UNIQUE, NOT NULL
password VARCHAR(255) NOT NULL (hashed)

🧑‍💻 Table : user
Attribut Type Contraintes
Id_user INT (AUTO) PK
first_name VARCHAR(50) NOT NULL
last_name VARCHAR(50) NOT NULL
email VARCHAR(50) UNIQUE, NOT NULL
password VARCHAR(255) NOT NULL
role VARCHAR(20) NOT NULL (coach/athlete/admin)
Id_admin INT FK → admin(Id_admin)
is_deleted BOOLEAN DEFAULT FALSE
is_active BOOLEAN DEFAULT TRUE
created_at DATETIME NOT NULL

Ajout de champs de sécurité : is_active, is_deleted, timestamps.

🧑‍🏋️‍♂️ Table : user_athlete
Attribut Type Contraintes
Id_user_athlete INT (AUTO) PK
weight_category VARCHAR(20) NOT NULL
next_competition_date DATE NULL
Id_subscription_paypal INT FK
Id_user INT UNIQUE, FK

🧑‍🏫 Table : user_coach
Attribut Type Contraintes
Id_user_coach INT (AUTO) PK
month_price DECIMAL NOT NULL
Id_subscription_stripe INT FK
Id_user INT UNIQUE, FK

💳 Table : subscription_paypal
Attribut Type Contraintes
Id_subscription_paypal INT (AUTO) PK
id_paypal VARCHAR(100) UNIQUE, NOT NULL
start_subscription DATE NOT NULL
end_subscription DATE NOT NULL
status_subscription VARCHAR(20) NOT NULL
price DECIMAL NOT NULL

💳 Table : subscription_stripe
Attribut Type Contraintes
Id_subscription_stripe INT (AUTO) PK
id_stripe VARCHAR(100) UNIQUE, NOT NULL
start_subscription DATE NOT NULL
end_subscription DATE NOT NULL
status_subscription VARCHAR(20) NOT NULL
price DECIMAL NOT NULL

🏦 Table : coach_payment_account
Attribut Type Contraintes
Id_coach_payment_account INT (AUTO) PK
payment_account_id VARCHAR(100) UNIQUE, NOT NULL
account_email VARCHAR(50) NOT NULL
status_account VARCHAR(20) NOT NULL
Id_user_coach INT FK
2️⃣ Ajout des tables nécessaires pour Tinode

Tinode gère les messages, canaux et utilisateurs. Nous devons donc ajouter 3 nouvelles tables dans ton modèle métier.

🆕 Table : tinode_user (équivalent Tinode du user)
Attribut Type Contraintes
Id_tinode_user INT (AUTO) PK
tinode_user_id VARCHAR(100) UNIQUE, NOT NULL
Id_user INT FK

Stocke la correspondance entre ton utilisateur interne et son intrant Tinode.

🆕 Table : tinode_channel
Attribut Type Contraintes
Id_tinode_channel INT (AUTO) PK
tinode_topic_name VARCHAR(100) UNIQUE, NOT NULL
Id_user_coach INT FK
Id_user_athlete INT FK
created_at DATETIME NOT NULL

Un canal = une conversation coach ↔ athlète.

🆕 Table : tinode_message
Attribut Type Contraintes
Id_tinode_message INT (AUTO) PK
tinode_message_id VARCHAR(100) UNIQUE, NOT NULL
Id_tinode_channel INT FK
Id_user INT FK
sent_at DATETIME NOT NULL
status VARCHAR(20) sent/delivered/read
