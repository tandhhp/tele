import MyCKEditor from "@/components/ckeditor";
import { apiAddCard, apiListCard, apiUpdateCard } from "@/services/user";
import { EditOutlined, PlusOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProColumnType, ProFormInstance, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components"
import { Button, Col, Row, Tag, message } from "antd";
import { useEffect, useRef, useState } from "react";

const CardPage: React.FC = () => {

    const [open, setOpen] = useState<boolean>();
    const actionRef = useRef<ActionType>();
    const formRef = useRef<ProFormInstance>();
    const [card, setCard] = useState<any>();
    const [loading, setLoading] = useState<boolean>(false);

    useEffect(() => {
        if (card) {
            setLoading(true);
            formRef.current?.setFields([
                {
                    name: 'id',
                    value: card.id
                },
                {
                    name: 'code',
                    value: card.code
                },
                {
                    name: 'tier',
                    value: card.tier
                },
                {
                    name: 'servicePrice',
                    value: card.servicePrice
                },
                {
                    name: 'content',
                    value: card.content
                },
                {
                    name: 'loyalty',
                    value: card.loyalty
                }
            ]);
            setLoading(false);
        }
    }, [card]);

    const columns: ProColumnType<any>[] = [
        {
            title: '#',
            valueType: 'indexBorder',
            width: 50
        },
        {
            title: 'Số chủ thẻ',
            dataIndex: 'users',
            width: 150,
            valueType: 'digit',
            search: false
        },
        {
            title: 'Hạng',
            dataIndex: 'tier',
            render: (_, entity) => <Tag color={entity.color} className='w-full text-center'>{entity.code}</Tag>,
            width: 100,
            search: false
        },
        {
            title: 'Số điểm',
            dataIndex: 'loyalty',
            valueType: 'digit',
            search: false
        },
        {
            title: 'Ngày cập nhật',
            dataIndex: 'modifiedDate',
            valueType: 'fromNow',
            search: false,
        },
        {
            title: 'Tác vụ',
            render: (dom, entity) => [
                <Button key="edit" icon={<EditOutlined />} type="primary" size="small" onClick={() => {
                    setCard(entity);
                    setOpen(true);
                }}></Button>
            ],
            valueType: 'option',
            width: 60,
        }
    ];

    const onFinish = async (values: any) => {
        if (values.id) {
            await apiUpdateCard(values);
        } else {
            await apiAddCard(values);
        }
        message.success('Thành công!');
        actionRef.current?.reload();
        setCard(null);
        formRef.current?.resetFields();
        setOpen(false);
    }

    return (
        <PageContainer extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)} hidden>Thêm thẻ</Button>}>
            <ProTable
                actionRef={actionRef}
                search={false}
                columns={columns}
                request={apiListCard}
            />
            <ModalForm open={open} onOpenChange={setOpen} title="Thẻ" onFinish={onFinish} formRef={formRef}>
                <ProFormText hidden name="id" />
                <ProFormText name="code" label="Mã thẻ" hidden disabled={card?.id} rules={[
                    {
                        required: true,
                        message: 'Vui lòng nhập mã thẻ'
                    }
                ]} />
                <Row gutter={16}>
                    <Col span={8}>
                        <ProFormSelect name="tier" disabled label="Hạng" rules={[
                            {
                                required: true,
                                message: 'Vui lòng chọn hạng'
                            }
                        ]} options={[
                            {
                                label: 'Standard',
                                value: 0
                            },
                            {
                                label: 'Elite',
                                value: 1
                            },
                            {
                                label: 'Royal',
                                value: 2
                            }
                        ]} />
                    </Col>
                    <Col span={16}>
                        <ProFormText name="loyalty" label="Số điểm" rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col span={24}>
                        <MyCKEditor loading={loading} label="Nội dung" name="content" />
                    </Col>
                </Row>
            </ModalForm>
        </PageContainer>
    )
}

export default CardPage;