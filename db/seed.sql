-- Insert users
INSERT INTO users ( username, password) VALUES
('john_doe', 'hashed_password_123'),
('jane_smith', 'hashed_password_456'),
('alice_jones', 'hashed_password_789');

-- Insert posts
INSERT INTO posts (user_id, content) VALUES
(1, 'Hello world! This is my first post.'),
(2, 'Hey everyone! Excited to be here.'),
(3, 'Good morning! What a beautiful day.'),
(1, 'Just had an amazing coffee today.'),
(2, 'Anyone up for a chat?');
