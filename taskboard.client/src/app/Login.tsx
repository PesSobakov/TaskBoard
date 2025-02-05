import { Button, Form } from 'react-bootstrap';
import  ApiService from '../Api/apiServce';
import { LoginDto } from '../DTOs/LoginDto';
import { useState } from 'react';
import { useNavigate } from 'react-router';

function Login()
{
    const [login, setLogin] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [error, setError] = useState<string>();
    const navigate = useNavigate();

    const GetUser = async () =>
    {
        const [response, responseError] = await ApiService.GetUser();
        if (responseError == null) {
            if (response?.value != null) {
                navigate('/account')
            }
        }
    }

    const LoginUser = async () =>
    {
        const dto: LoginDto = { login: login, password: password };
        const [, responseError] = await ApiService.Login(dto);
        if (responseError == null) {
            navigate('/account')
        }
        else {
            setError(responseError);
        }
    }

    GetUser();

    const errorElement = (
        <Form.Text className="text-muted">
            {error}
        </Form.Text>
    );

    return (
        <div>
            <Form>
                <Form.Group className="mb-3" controlId="formBasicEmail">
                    <Form.Label>Login</Form.Label>
                    <Form.Control type="email" placeholder="Login" value={login} onChange={(e) => { setLogin(e.target.value) }} />
                </Form.Group>
                <Form.Group className="mb-3" controlId="formBasicPassword">
                    <Form.Label>Password</Form.Label>
                    <Form.Control type="password" placeholder="Password" value={password} onChange={(e) => { setPassword(e.target.value) }} />
                </Form.Group>
                <Button type="button" variant="primary" onClick={LoginUser}>
                    Login
                </Button>
                {error !== undefined ? errorElement :null}
            </Form>
        </div>
    );
}

export default Login;