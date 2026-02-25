const authBaseUrl = "https://127.0.0.1:7261/api/auth";

// Access token lives in memory only
let accessToken = null;

// -- API functions --
const login = async (identifier, password) => {
  const loginUrl = authBaseUrl + "/login";

  try {
    const response = await fetch(loginUrl, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(new LoginRequest(identifier, password)),
      credentials: "include",
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

const register = async (username, email, password) => {
  const registerUrl = authBaseUrl + "/register";

  try {
    const response = await fetch(registerUrl, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(new RegisterRequest(username, email, password)),
      credentials: "include",
    });

    if (!response.ok) {
      const err = await response.json();
      alert(`Register failed: ${err.description}`);
      return;
    }

    alert("Registration successful! Please log in.");
    window.location.href = "login.html";
  } catch (err) {
    console.error(err);
    alert("Registration error");
  }
};

// Silently attempts to get a new access token using the refresh cookie.
// Returns true if successful, false if not
const tryRefresh = async () => {
  const refreshUrl = authBaseUrl + "/refresh";

  try {
    const response = await fetch(refreshUrl, {
      method: "POST",
      credentials: "include",
    });

    if (response.ok) {
      const data = await response.json();
      accessToken = data.token;
      return true;
    }

    accessToken = null;
    return false;
  } catch (err) {
    console.error(err);
    accessToken = null;
    return false;
  }
};

const aboutMe = async () => {
  const aboutMeUrl = authBaseUrl + "/test-me";

  try {
    const response = await fetch(aboutMeUrl, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
      credentials: "include",
    });

    if (response.ok) {
      const data = await response.json();

      const welcomeEl = document.getElementById("welcome");
      const userInfoEl = document.getElementById("user-info");

      if (welcomeEl) welcomeEl.innerText = `Welcome, ${data.username}!`;
      if (userInfoEl)
        userInfoEl.innerHTML = `
        <p><strong>ID:</strong> ${data.id}</p>
        <p><strong>Username:</strong> ${data.username}</p>
        <p><strong>Email:</strong> ${data.email}</p>
        `;
    } else {
      // Unauthorized, redirect to home
      window.location.href = "index.html";
    }
  } catch (err) {
    console.error(err);
    window.location.href = "index.html";
  }
};

// -- Requests --

function LoginRequest(identifier, password) {
  this.identifier = identifier;
  this.password = password;
}

function RegisterRequest(username, email, password) {
  this.username = username;
  this.email = email;
  this.password = password;
}

// -- UI helpers --

function setLoggedInUI(user) {
  if (document.getElementById("loginBtn"))
    document.getElementById("loginBtn").style.display = "none";
  if (document.getElementById("registerBtn"))
    document.getElementById("registerBtn").style.display = "none";
  if (document.getElementById("myPageBtn"))
    document.getElementById("myPageBtn").style.display = "inline-block";
}

function setLoggedOutUI() {
  if (document.getElementById("loginBtn"))
    document.getElementById("loginBtn").style.display = "inline-block";
  if (document.getElementById("registerBtn"))
    document.getElementById("registerBtn").style.display = "inline-block";
  if (document.getElementById("myPageBtn"))
    document.getElementById("myPageBtn").style.display = "none";
}

// -- Bootstrap --

document.addEventListener("DOMContentLoaded", async () => {
  // On every page load, try a silent refresh to restore the access token from the cookie
  const isLoggedIn = await tryRefresh();

  if (isLoggedIn) {
    setLoggedInUI();
  } else {
    setLoggedOutUI();
  }

  // My Page: requires auth, load user info
  if (document.body.id === "my-page") {
    if (!isLoggedIn) {
      window.location.href = "index.html";
      return;
    }
    await aboutMe();
  }

  // Login form
  const loginForm = document.getElementById("login-form");
  if (loginForm) {
    loginForm.addEventListener("submit", async (e) => {
      e.preventDefault();
      const identifier = document.getElementById("identifier").value;
      const password = document.getElementById("password").value;
      await login(identifier, password);
    });
  }

  // Register form
  const registerForm = document.getElementById("register-form");
  if (registerForm) {
    registerForm.addEventListener("submit", async (e) => {
      e.preventDefault();
      const username = document.getElementById("username").value;
      const email = document.getElementById("email").value;
      const password = document.getElementById("password").value;
      await register(username, email, password);
    });
  }
});
