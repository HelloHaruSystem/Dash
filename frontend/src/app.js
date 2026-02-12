const authBaseUrl = "https://127.0.0.1:7261/api/auth";

let accessToken = null;

// functions

const login = async (identifier, password) => {
  const loginUrl = authBaseUrl + "/login";
  const request = new LoginRequest(identifier, password);

  try {
    const response = await fetch(loginUrl, {
      method: "POST",
      headers: {
        "accept": "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
      credentials: "include"
    });

    if (!response.ok) {
      const err = await response.json();
      alert(`Login failed: ${err.description}`);
      return;
   }

    const data = await response.json();
    accessToken = data.token;
    alert("Login successful!");
    window.location.href = "mypage.html";
  } catch (err) {
    console.error(err);
    alert("Login error");
  }
};

const register = () => {
  const registerUrl = authBaseUrl + "/register";
};

const refreshToken = () => {
  const refreshUrl = authBaseUrl + "/refresh";
};

const aboutMe = async () => {
  const aboutMeUrl = () => authBaseUrl + "/test/me";

  try {
    const response = await fetch(aboutMeUrl, {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${accessToken}`
      },
      credentials: "include"
    });

    if (response.ok) {
      // TODO Display information about the user from the response
      const data = await response.json();
      document.getElementById("Welcome").innerText = `Welcome ${data.username}!`;
    } else {
      // Unauthorized, redirect to home
      window.location.href = "index.html";
    }
  } catch (err) {
    console.error(err);
    window.location.href = "index.html";
  }
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

function Error(code, description) {
  this.code = code;
  this.description = description;
}

// event listeners
document.addEventListener("DOMContentLoaded", async () => {
  if (document.body.id === "home-page") {
    await aboutMe();
  }
});