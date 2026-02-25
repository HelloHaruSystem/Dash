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
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
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
  const request = new RegisterRequest(username, email, password);

  try {
    const response = await fetch(registerUrl, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
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

const refreshToken = async () => {
  const refreshUrl = authBaseUrl + "/refresh";

  try {
    const response = await fetch(refreshUrl, {
      method: "POST",
      credentials: "include",
    });

    if (response.ok) {
      const data = await response.json();
      accessToken = data.token;
      console.log("Token refreshed!");
    } else {
      accessToken = null;
      window.location.href = "login.html";
    }
  } catch (err) {
    console.error(err);
  }
};

const aboutMe = async () => {
  const aboutMeUrl = () => authBaseUrl + "/test/me";

  try {
    const response = await fetch(aboutMeUrl, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
      credentials: "include",
    });

    if (response.ok) {
      // TODO Display information about the user from the response
      const data = await response.json();
      document.getElementById("Welcome").innerText =
        `Welcome ${data.username}!`;
    } else {
      // Unauthorized, redirect to home
      window.location.href = "index.html";
    }
  } catch (err) {
    console.error(err);
    window.location.href = "index.html";
  }
};

async function checkAuth() {
  const aboutMeUrl = () => authBaseUrl + "/test/me";

  try {
    const response = await fetch(aboutMeUrl, {
      method: "GET",
      headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : {},
      credentials: "include",
    });

    if (response.ok) {
      const data = await response.json();
      setLoggedInUI(data);
      return true;
    } else {
      setLoggedOutUI();
      return false;
    }
  } catch {
    setLoggedOutUI();
    return false;
  }
}

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

function setLoggedInUI(user) {
  document.getElementById("loginBtn")?.style.display = "none";
  document.getElementById("registerBtn")?.style.display = "none";
  document.getElementById("myPageBtn")?.style.display = "inline-block";
}

function setLoggedOutUI() {
  document.getElementById("loginBtn")?.style.display = "inline-block";
  document.getElementById("registerBtn")?.style.display = "inline-block";
  document.getElementById("myPageBtn")?.style.display = "none";
}

// event listeners
document.addEventListener("DOMContentLoaded", async () => {
  if (document.body.id === "home-page") {
    await aboutMe();
  } else {
    document.addEventListener("DOMContentLoaded", () => {
      checkAuth();
    });
  }
});
