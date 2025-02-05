import { Button, Form } from 'react-bootstrap';
import ApiService from '../Api/apiServce';
import { useState } from 'react';
import { Link } from 'react-router';

function Login()
{
    const [user, setUser] = useState<string | null>("");

    const GetUser = async () =>
    {
        const [response,] = await ApiService.GetUser();
        setUser(response?.value ?? null);
    }

    const Logout = async () =>
    {
        const [,] = await ApiService.Logout();
        GetUser();
    }

    const DeleteAccount = async () =>
    {
        const [,] = await ApiService.DeleteAccount();
        GetUser();
    }

    GetUser();

    return (
        <div>
            {user == null ?
                <div>
                    You not logged in. <Link to="/login">Register</Link> or <Link to="/login">Log in.</Link>
                </div>
                :
                <Form>
                    <Form.Group className="mb-3">
                        <Form.Text>
                            You logged as {user}
                        </Form.Text>
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Button type="button" variant="primary" onClick={Logout}>
                            Logout
                        </Button>
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Button type="button" onClick={DeleteAccount}>
                            Delete Account
                        </Button>
                    </Form.Group>
                </Form>
            }
        </div>
    );
}

export default Login;