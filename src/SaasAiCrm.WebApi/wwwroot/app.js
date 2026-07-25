const form = document.querySelector("#loginForm");
const message = document.querySelector("#message");
const submitButton = document.querySelector("#submitButton");
const password = document.querySelector("#password");

document.querySelector("#togglePassword").addEventListener("click", (event) => {
  const visible = password.type === "text";
  password.type = visible ? "password" : "text";
  event.currentTarget.textContent = visible ? "Göster" : "Gizle";
});

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  message.textContent = "";
  submitButton.disabled = true;
  submitButton.textContent = "Giriş yapılıyor…";

  try {
    const response = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        email: form.email.value.trim(),
        password: form.password.value
      })
    });

    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.detail || "E-posta veya parola hatalı.");
    }

    sessionStorage.setItem("accessToken", result.accessToken);
    sessionStorage.setItem("currentUser", JSON.stringify(result.user ?? {}));
    window.location.assign("/dashboard.html");
  } catch (error) {
    message.textContent = error.message || "Giriş sırasında bir hata oluştu.";
  } finally {
    submitButton.disabled = false;
    submitButton.textContent = "Giriş yap";
  }
});
