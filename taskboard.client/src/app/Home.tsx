import { BoardDto } from '../DTOs/TaskBoard/BoardDto';
import { useEffect, useState } from 'react';
import ApiService from '../Api/apiServce';
import { Privatness } from '../DTOs/TaskBoard/Privatness';
import { Button, Card as BootstrapCard, Form } from 'react-bootstrap';
import { CreateBoardDto } from '../DTOs/CreateBoardDto';

function Home()
{
    const [boards, setBoards] = useState<BoardDto[]>();
    const [error, setError] = useState<string>();
    const [name, setName] = useState<string>("");

    const renderPrivatness = (privatness: Privatness): string =>
    {
        switch (privatness) {
            case Privatness.Private:
                return "Private"
            case Privatness.Public:
                return "Public"
        }
    }

    const updateBoards = async () =>
    {
        const [response, responseError] = await ApiService.GetBoards();
        if (responseError == null) {
            setError(undefined);
            if (response != null) {
                setBoards(response);
            }
            else {
                setBoards(undefined);
                setError("BadRequest");
            }
        }
        else {
            setError(responseError);
            return;
        }
    }

    const deleteBoard = async (id: number) =>
    {
        const [, responseError] = await ApiService.DeleteBoard(id);
        if (responseError == null) {
            updateBoards();
        }
    }

    const createBoard = async () =>
    {
        const dto: CreateBoardDto = { name: name, description: "" };
        const [, responseError] = await ApiService.CreateBoard(dto);
        if (responseError == null) {
            updateBoards();
        }
    }


    useEffect(() =>
    {
        updateBoards();
    }, []);

    let counter = 0;

    const boardsElements = boards?.map((board) =>
    {
        const href = `/board/${board.id}`;
        return (
            <div key={counter++}>
                <BootstrapCard style={{ width: '20rem' }}>
                    <BootstrapCard.Body>
                        <BootstrapCard.Title>
                            <span style={{ display: "block", width: '13rem' }}>
                                {board.name}
                            </span>
                        </BootstrapCard.Title>
                        <BootstrapCard.Subtitle className="mb-2 text-muted">
                            {renderPrivatness(board.privatness)}
                        </BootstrapCard.Subtitle>
                        <BootstrapCard.Text as="div">
                            {board.description}
                        </BootstrapCard.Text>
                        <BootstrapCard.Text as="div">
                            <Button href={href}>Open</Button>
                        </BootstrapCard.Text>
                        <BootstrapCard.Text as="div">
                            <Button onClick={() => { deleteBoard(board.id) }}>Delete</Button>
                        </BootstrapCard.Text>
                    </BootstrapCard.Body>
                </BootstrapCard>
            </div>
        );
    })

    return (
        <div>
            <div>Home</div>
            {boards != undefined ?
                <div>
                    <div>
                        {boardsElements}
                    </div>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Name</Form.Label>
                            <Form.Control type="text" placeholder="Name" value={name} onChange={(e) => { setName(e.target.value) }} />
                        </Form.Group>
                        <Button type="button" variant="primary" onClick={createBoard}>
                            Create board
                        </Button>
                    </Form>
                </div>
                : null
            }
            <div>
                {error}
            </div>
        </div>
    );
}

export default Home;