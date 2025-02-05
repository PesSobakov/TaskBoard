import { CardDto } from '../DTOs/TaskBoard/CardDto';
import ApiService from '../Api/apiServce';
import { MoveCardDto } from '../DTOs/MoveCardDto';
import { ChangeCardOrderDto } from '../DTOs/ChangeCardOrderDto';
import { useNavigate } from 'react-router';
import { BoardDto } from '../DTOs/TaskBoard/BoardDto';
import { Button, Dropdown, Card as BootstrapCard } from 'react-bootstrap';
import { Calendar, Chat, ThreeDots } from 'react-bootstrap-icons';
import "./Card.css";
import { useState } from 'react';
interface CardProps
{
    board: BoardDto;
    card: CardDto;
    update: () => void;
}
function Card({ board, card, update }: CardProps)
{
    const [menuShow, setMenuShow] = useState(false);
    const renderDate = (date: Date) =>
    {
        return new Date(date).toISOString().slice(0, 10)
    }

    const deleteCard = async () =>
    {
        const [, responseError] = await ApiService.DeleteCard(card.id);
        if (responseError == null) {
            update();
        }
    }

    const moveCard = async (id: number) =>
    {
        const dto: MoveCardDto = { listId: id };
        const [, responseError] = await ApiService.MoveCard(card.id, dto);
        if (responseError == null) {
            update();
        }
    }

    const changeCardOrder = async (order: number) =>
    {
        const dto: ChangeCardOrderDto = { order: order };
        const [, responseError] = await ApiService.ChangeCardOrder(card.id, dto);
        if (responseError == null) {
            update();
        }
    }

    const navigate = useNavigate();
    const openCard = () =>
    {
        navigate(`/board/${board.id}/card/${card.id}`)
    }

    const list = board.lists.find((x) =>
    {
        return x.cards.find((y) =>
        {
            return y.id == card.id
        })
    });
    const listId = list?.id;
    let counter = 0;
    const moveCardLists = board.lists.map(list =>
    {
        if (list.id == listId) {
            return <Dropdown.Item key={`move-card-lists-${list.id}-${counter++}`} disabled>{list.name}</Dropdown.Item>
        }
        else {
            return <Dropdown.Item key={`move-card-lists-${list.id}-${counter++}`} onClick={(e) => { e.stopPropagation(); moveCard(list.id); setMenuShow(false); }}>{list.name}</Dropdown.Item>
        }
    })
    const changeCardOrderOrders = list?.cards.map(c =>
    {
        if (c.id == card.id) {
            return <Dropdown.Item key={`change-card-order-${list.id}-${counter++}`} disabled>{c.order}</Dropdown.Item>
        }
        else {
            return <Dropdown.Item key={`change-card-order-${list.id}-${counter++}`} onClick={(e) => { e.stopPropagation(); changeCardOrder(c.order); setMenuShow(false); }}>{c.order}</Dropdown.Item>
        }
    })


    return (
        <div>
            <BootstrapCard style={{ width: '20rem' }}>
                <BootstrapCard.Body>
                    <BootstrapCard.Title>
                        <span style={{ display: "block", width: '13rem' }}>
                            {card.name}
                        </span>
                        <span style={{ display: "block", width: '4rem' }}>
                            <Dropdown show={menuShow} onToggle={() => setMenuShow (!menuShow) }>
                                <Dropdown.Toggle variant="secondary" className="b-transparent dropdown-toggle-disable">
                                    <ThreeDots />
                                </Dropdown.Toggle>
                                <Dropdown.Menu>
                                    <Button variant="secondary" className="w-100 b-transparent" onClick={() => { deleteCard(); setMenuShow(false); } }>Delete</Button>
                                    <Dropdown drop='end'>
                                        <Dropdown.Toggle variant="secondary" className="w-100 b-transparent">
                                            Move list
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu>
                                            {moveCardLists}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                    <Dropdown drop='end'>
                                        <Dropdown.Toggle variant="secondary" className="w-100 b-transparent">
                                            Change order
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu>
                                            {changeCardOrderOrders}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </Dropdown.Menu>
                            </Dropdown>
                        </span>
                    </BootstrapCard.Title>
                    <BootstrapCard.Subtitle className="mb-2 text-muted">
                        {card.status}
                    </BootstrapCard.Subtitle>
                    <BootstrapCard.Text as="div">
                        <span className="p-1">
                            <span className="p-1">
                                <Calendar />
                            </span>
                            <span className="p-1">
                                {renderDate(card.dueDate)}
                            </span>
                        </span>
                        <span className="p-1">
                            <span className="p-1">
                                <Chat />
                            </span>
                            <span className="p-1">
                                {card.comments.length}
                            </span>
                        </span>
                    </BootstrapCard.Text>
                    <BootstrapCard.Text as="div">
                        <div><Button onClick={openCard}>Open</Button></div>
                    </BootstrapCard.Text>
                </BootstrapCard.Body>
            </BootstrapCard>
        </div>
    );
}

export default Card;