import { BoardDto } from '../DTOs/TaskBoard/BoardDto';
import { ListDto } from '../DTOs/TaskBoard/ListDto';
import { Button, Dropdown, Card as BootstrapCard, Form } from 'react-bootstrap';
import Card from './Card';
import { EditListDto } from '../DTOs/EditListDto';
import { useState } from 'react';
import { ThreeDots } from 'react-bootstrap-icons';
import ApiService from '../Api/apiServce';
import { ChangeListOrderDto } from '../DTOs/ChangeListOrderDto';
import { CreateCardDto } from '../DTOs/CreateCardDto';

interface ListProps
{
    board: BoardDto;
    list: ListDto;
    update: () => void;
}

function List({ board, list, update }: ListProps)
{
    const [menuShow, setMenuShow] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [name, setName] = useState<string>("");
    const [error, setError] = useState<string>();
    const [cardName, setCardName] = useState<string>("");

    const deleteList = async () =>
    {
        const [, responseError] = await ApiService.DeleteList(list.id);
        if (responseError == null) {
            update();
        }
    }

    const changeListOrder = async (order: number) =>
    {
        const dto: ChangeListOrderDto = { order: order };
        const [, responseError] = await ApiService.ChangeListOrder(list.id, dto);
        if (responseError == null) {
            update();
        }
    }

    const editList = async () =>
    {
        const dto: EditListDto = { name: name };
        const [, responseError] = await ApiService.EditList(list.id, dto);
        if (responseError == null) {
            setIsEditing(false);
            update();
        }
        else {
            setError(responseError);
        }
    }

    const createCard = async () =>
    {
        const dto: CreateCardDto = { name: cardName ,listId:list.id,description:"",status:"",dueDate : undefined!};
        const [, responseError] = await ApiService.CreateCard(dto);
        if (responseError == null) {
            update();
        }
    }


    const startEditing = () =>
    {
        setIsEditing(true);
        setName(list.name);
    }


    const cards = list.cards.sort((a, b) => a.order - b.order).map((c) => { return <Card key={`card-${c.id}`} board={board} card={c} update={update} /> });

    let counter = 0;
    const changeListOrderOrders = board?.lists.map(l =>
    {
        if (l.id == list.id) {
            return <Dropdown.Item key={`change-list-order-${list.id}-${counter++}`} disabled>{l.order}</Dropdown.Item>
        }
        else {
            return <Dropdown.Item key={`change-list-order-${list.id}-${counter++}`} onClick={(e) => { e.stopPropagation(); changeListOrder(l.order); setMenuShow(false); }}>{l.order}</Dropdown.Item>
        }
    })


    return (
        <div>
            <BootstrapCard style={{/* width: '20rem' */ }}>
                <BootstrapCard.Body>
                    <BootstrapCard.Title>
                        {isEditing == true ?
                            <div>
                                <Form>
                                    <Form.Group className="mb-3">
                                        <Form.Control type="text" placeholder="Name" value={name} onChange={(e) => { setName(e.target.value) }} />
                                    </Form.Group>
                                    <Button type="button" variant="primary" onClick={editList}>
                                        Confirm
                                    </Button>
                                    <Button type="button" variant="primary" onClick={() => { setIsEditing(false) }}>
                                        Cancel
                                    </Button>
                                    {error !== undefined ? <div>{error}</div> : null}
                                </Form>
                            </div>
                            :
                            <div>
                                <span style={{ display: "block", width: '13rem' }}>
                                    {list.name}
                                </span>
                                <span style={{ display: "block", width: '4rem' }}>
                                    <Dropdown show={menuShow} onToggle={() => setMenuShow(!menuShow)}>
                                        <Dropdown.Toggle variant="secondary" className="b-transparent dropdown-toggle-disable">
                                            <ThreeDots />
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu>
                                            <Button variant="secondary" className="w-100 b-transparent" onClick={() => { deleteList(); setMenuShow(false); }}>Delete</Button>
                                            <Dropdown drop='end'>
                                                <Dropdown.Toggle variant="secondary" className="w-100 b-transparent">
                                                    Change order
                                                </Dropdown.Toggle>
                                                <Dropdown.Menu>
                                                    {changeListOrderOrders}
                                                </Dropdown.Menu>
                                            </Dropdown>
                                            <Button variant="secondary" className="w-100 b-transparent" onClick={() => { startEditing(); setMenuShow(false); }}>Edit</Button>
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </span>
                            </div>
                        }
                    </BootstrapCard.Title>
                    <BootstrapCard.Text as="div">
                        <div>{cards}</div>
                        <div>
                            <Form>
                                <Form.Group className="mb-3">
                                    <Form.Label>Name</Form.Label>
                                    <Form.Control type="text" placeholder="Name" value={cardName} onChange={(e) => { setCardName(e.target.value) }} />
                                </Form.Group>
                                <Button type="button" variant="primary" onClick={createCard}>
                                    Create card
                                </Button>
                            </Form>
                        </div>
                    </BootstrapCard.Text>
                </BootstrapCard.Body>
            </BootstrapCard>
        </div>
    );
}

export default List;