CREATE TABLE subscription_app_stripe (
    id_subscription_app_stripe INT IDENTITY(1,1) PRIMARY KEY,
    id_stripe VARCHAR(50) NOT NULL,
    start_subscription DATE NOT NULL,
    end_subscription DATE NOT NULL,
    status_subscription VARCHAR(20) NOT NULL,
    price DECIMAL(15,2) NOT NULL,
    created_at DATE NOT NULL
);

CREATE TABLE subscription_coach_stripe (
    id_coach_subscription_stripe INT IDENTITY(1,1) PRIMARY KEY,
    id_stripe VARCHAR(50) NOT NULL,
    start_subscription DATE NOT NULL,
    end_subscription DATE NOT NULL,
    status_subscription VARCHAR(20) NOT NULL,
    price DECIMAL(15,2) NOT NULL,
    created_at DATE NOT NULL
);

CREATE TABLE User_(
    Id_user INT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(20) NOT NULL,
    last_name VARCHAR(20) NOT NULL,
    email VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    role_admin BIT NOT NULL,
    id_subscription_app_stripe INT NOT NULL,
    FOREIGN KEY(id_subscription_app_stripe) REFERENCES subscription_app_stripe(id_subscription_app_stripe)
);

CREATE TABLE user_block(
    id_block INT IDENTITY(1,1) PRIMARY KEY,
    id_user_blocker INT NOT NULL,
    id_user_blocked INT NOT NULL,
    status BIT,
    reason VARCHAR(50),
    created_at DATETIME NOT NULL,
    FOREIGN KEY(id_user_blocker) REFERENCES User_(Id_user),
    FOREIGN KEY(id_user_blocked) REFERENCES User_(Id_user)
);

CREATE TABLE user_report(
    id_report INT IDENTITY(1,1) PRIMARY KEY,
    reason VARCHAR(200) NOT NULL,
    created_at DATETIME NOT NULL,
    status VARCHAR(20) NOT NULL,
    id_user_reporter INT NOT NULL,
    id_user_reported INT NOT NULL,
    FOREIGN KEY(id_user_reporter) REFERENCES User_(Id_user),
    FOREIGN KEY(id_user_reported) REFERENCES User_(Id_user)
);

CREATE TABLE user_athlete(
    id_user_athlete INT IDENTITY(1,1) PRIMARY KEY,
    weight_category VARCHAR(20) NOT NULL,
    id_coach_subscription_stripe INT NOT NULL,
    Id_user INT NOT NULL UNIQUE,
    FOREIGN KEY(id_coach_subscription_stripe) REFERENCES subscription_coach_stripe(id_coach_subscription_stripe),
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user)
);

CREATE TABLE user_coach(
    id_user_coach INT IDENTITY(1,1) PRIMARY KEY,
    month_price DECIMAL(15,2) NOT NULL,
    biography TEXT NOT NULL,
    specialities VARCHAR(200) NOT NULL,
    completed_sessions INT NOT NULL,
    rating DECIMAL(3,2) NOT NULL,
    id_coach_subscription_stripe INT NOT NULL,
    Id_user INT NOT NULL UNIQUE,
    FOREIGN KEY(id_coach_subscription_stripe) REFERENCES subscription_coach_stripe(id_coach_subscription_stripe),
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user)
);

CREATE TABLE user_session(
    id_session INT IDENTITY(1,1) PRIMARY KEY,
    device_type VARCHAR(50) NOT NULL,
    ip_adress VARCHAR(50) NOT NULL,
    created_at DATETIME NOT NULL,
    expires_at DATETIME NOT NULL,
    Id_user INT,
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user)
);

CREATE TABLE user_refresh_token(
    id_refresh INT IDENTITY(1,1) PRIMARY KEY,
    refresh_token VARCHAR(250) NOT NULL,
    expires_at DATETIME NOT NULL,
    Id_user INT,
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user)
);

CREATE TABLE tinode_account(
    id_tinode_account INT IDENTITY(1,1) PRIMARY KEY,
    tinode_user_id VARCHAR(250) NOT NULL,
    created_at DATETIME NOT NULL,
    Id_user INT,
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user)
);

CREATE TABLE audit_log(
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    action VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    created_at DATETIME NOT NULL,
    Id_user INT,
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user)
);

CREATE TABLE report(
    Id_user INT,
    id_report INT,
    PRIMARY KEY(Id_user, id_report),
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user),
    FOREIGN KEY(id_report) REFERENCES user_report(id_report)
);

CREATE TABLE link(
    id_user_athlete INT,
    id_user_coach INT,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status VARCHAR(50) NOT NULL,
    PRIMARY KEY(id_user_athlete, id_user_coach),
    FOREIGN KEY(id_user_athlete) REFERENCES user_athlete(id_user_athlete),
    FOREIGN KEY(id_user_coach) REFERENCES user_coach(id_user_coach)
);

CREATE TABLE blocage(
    Id_user INT,
    id_block INT,
    PRIMARY KEY(Id_user, id_block),
    FOREIGN KEY(Id_user) REFERENCES User_(Id_user),
    FOREIGN KEY(id_block) REFERENCES user_block(id_block)
);
