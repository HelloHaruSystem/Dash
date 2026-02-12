const authBaseUrl = "https://127.0.0.1:7261/api/auth";

const login = async (identifier, password) => {
  const loginUrl = authBaseUrl + "/login";
  const request = new LoginRequest(identifier, password);

  const response = await fetch(loginUrl, {
    method: "POST",
    headers: {
      accept: "application/json",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });
};

const register = () => {
  const registerUrl = authBaseUrl + "/register";
};

const refresh = () => {
  const refreshUrl = authBaseUrl + "/refresh";
};

const aboutMe = () => {
  const aboutMeUrl = () => authBaseUrl + "/test/me";
};

function AuthResponse(id, username, email, token, refreshToken) {
  this.id = id;
  this.username = username;
  this.email = email;
  this.token = token;
  this.refreshToken = refreshToken;
}

function LoginRequest(identifier, password) {
  this.identifier = identifier;
  this.password = password;
}

function RegisterRequest(username, email, password) {
  this.username = username;
  this.email = email;
  this.password = password;
}

function RefreshRequest(refreshToken) {
  this.refreshToken = refreshToken;
}

function Error(code, description) {
  this.code = code;
  this.description = description;
}
