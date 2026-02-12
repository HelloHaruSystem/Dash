const login = () => {};

const register = () => {};

const AuthResponse = (id, username, email, token, refreshToken) => {
  this.id = id;
  this.username = username;
  this.email = email;
  this.token = token;
  this.refreshToken = refreshToken;
};

const LoginRequest = (identifier, password) => {};

const RegisterRequest = (username, email, password) => {};

const RefreshRequest = (refreshToken) => {};

const Error = (code, description) => {};
