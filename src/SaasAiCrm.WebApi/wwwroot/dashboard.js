const token = sessionStorage.getItem("accessToken");

if (!token) {
  window.location.replace("/");
} else {
  loadCurrentUser();
}

async function loadCurrentUser() {
  try {
    const response = await fetch("/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` }
    });

    if (!response.ok) {
      throw new Error("Oturum geçersiz.");
    }

    const user = await response.json();
    const fullName = user.fullName || "Kullanıcı";
    document.querySelector("#userName").textContent = fullName;
    document.querySelector("#avatar").textContent = fullName.charAt(0).toLocaleUpperCase("tr-TR");
  } catch {
    clearSessionAndRedirect();
  }
}

document.querySelector("#logoutButton").addEventListener("click", clearSessionAndRedirect);

function clearSessionAndRedirect() {
  sessionStorage.removeItem("accessToken");
  sessionStorage.removeItem("currentUser");
  window.location.replace("/");
}
