-- Create the 'users' table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,      -- Unique ID for each user
    username VARCHAR(50) NOT NULL UNIQUE,  -- Unique username
    password VARCHAR(255) NOT NULL,  -- Store hashed password for security
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP  -- Timestamp of registration
);

-- Create the 'posts' table
CREATE TABLE posts (
    id SERIAL PRIMARY KEY,      -- Unique ID for each post
    user_id INT REFERENCES users(id) ON DELETE CASCADE,  -- Reference to the user who made the post
    content TEXT NOT NULL,      -- Post content
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP  -- Timestamp of the post
);

-- Optional: Create an index on the 'created_at' field of 'posts' table for faster ordering
CREATE INDEX idx_posts_created_at ON posts(created_at);
