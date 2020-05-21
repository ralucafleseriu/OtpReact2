import React, { useState, useEffect } from 'react';

export const Home = () => {

  const [password, setPassword] = useState("");
  const [userId, setUserId] = useState("");
  const [expirationDateTime, setExpirationDateTime] = useState();
  const [counter, setCounter] = useState();
  const [myInterval, setMyInterval] = useState();

  async function handleGeneratePassword() {
    const response = await fetch('onetimepassword');
    const data = await response.json();
    setPassword(data.password);
    setUserId(data.userId);
    setExpirationDateTime(new Date(data.expirationDateTime));

  }

  async function handleValidatePassword(password) {
    const response = await fetch('onetimepassword/validate/' + password, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(userId)
    });
    const data = await response.json();
    alert(data ? "Password valid!" : "Password expired... :( ");
    if (!data) {
      setPassword();
      setCounter()
      setExpirationDateTime();
    }
  }

  useEffect(() => {
    clearInterval(myInterval);
    setMyInterval()

    if (!expirationDateTime) return;

    const id = setInterval(() => {
      if (expirationDateTime) {
        var c = Math.floor((expirationDateTime - new Date()) / 1000);
        setCounter(c)
      }
    }, 1000);
    setMyInterval(id);

    return () => {
      clearInterval(myInterval);
      setMyInterval()
    }
    // eslint-disable-next-line 
  }, [expirationDateTime]);

  function getPasswordStatus() {
    if (!counter) return null;
    if (counter <= 0) {
      return "Password expired.";
    }
    return `Valid for ${counter} seconds.`;
  }

  return (
    <div className="home">
      <h1>Home</h1>
      <input value={password} />
      <button onClick={() => handleGeneratePassword()}>Generate one time password</button>
      <p>{getPasswordStatus()}</p>
      <button onClick={() => handleValidatePassword(password)}>Validate password</button>
    </div >
  );
}
