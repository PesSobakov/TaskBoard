import { useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import { useNavigate } from 'react-router';
import ApiService from '../Api/apiServce';
import { RegisterDto } from '../DTOs/RegisterDto';

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

    const RegisterUser = async () =>
    {
        const dto: RegisterDto = { login: login, password: password };
        const [, responseError] = await ApiService.Register(dto);
        if (responseError == null) {
            setError(undefined);
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
                <Button type="button" variant="primary" onClick={RegisterUser}>
                    Register
                </Button>
                {error !== undefined ? errorElement : null}
            </Form>
        </div>
    );
}

export default Login;