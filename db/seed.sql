-- Insert users
INSERT INTO users (username, password, email) VALUES
('john_doe', 'hashed_password_123', 'john@example.com'),
('jane_smith', 'hashed_password_456', 'jane@example.com'),
('alice_jones', 'hashed_password_789', 'alice@example.com');

-- Insert posts
INSERT INTO posts (user_id, content) VALUES
(1, 'Hello world! This is my first post.'),
(2, 'Hey everyone! Excited to be here.'),
(3, 'Good morning! What a beautiful day.'),
(1, 'Just had an amazing coffee today.'),
(2, 'Anyone up for a chat?');
